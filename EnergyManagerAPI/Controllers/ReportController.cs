using EnergyManagerCore.Extentions;
using EnergyManagerCore.Services;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql.Internal;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Document = iText.Layout.Document;
using Table = iText.Layout.Element.Table;

namespace EnergyManagerWeb.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GeneratePdfReport()
        {
            var userId = User.GetUserId();
            var reportData = await _reportService.GenerateReportDataAsync(userId);

            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph("Energy Consumption Report").SetFontSize(16));

            foreach (var dm in reportData.DeviceMeasurements)
            {
                document.Add(new Paragraph($"Device: {dm.Device.Name} ({dm.Device.Type}) - Total Consumption: {dm.Total} kWh"));

                if (dm.Measurements.Any())
                {
                    var table = new Table(4);
                    table.AddHeaderCell("ID");
                    table.AddHeaderCell("Timestamp");
                    table.AddHeaderCell("Value");
                    table.AddHeaderCell("Unit");

                    foreach (var m in dm.Measurements)
                    {
                        table.AddCell(m.Id.ToString());
                        table.AddCell(m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                        table.AddCell(m.Value.ToString());
                        table.AddCell(m.Unit);
                    }

                    document.Add(table);
                }
                else
                {
                    document.Add(new Paragraph("No measurements available."));
                }
            }

            document.Add(new Paragraph($"Grand Total Consumption (All Time): {reportData.GrandTotal} kWh"));
            document.Add(new Paragraph($"Total Consumption (Last Month): {reportData.MonthlyTotal} kWh"));

            document.Close();

            var bytes = memoryStream.ToArray();
            return File(bytes, "application/pdf", $"energy_report_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
    }
}
