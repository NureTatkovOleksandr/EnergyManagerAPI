using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Sqlite;
namespace EnergyManagerCore.Data
{
    using EnergyManagerCore.Models;
    using Microsoft.EntityFrameworkCore;

    // ✅ **Виправлений ApplicationDbContext.cs** (Копіюй + Запусти!)
    using Microsoft.EntityFrameworkCore;  // **✅ using для UseSqlite**

    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<House> Houses { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<Measurement> Measurements { get; set; } = null!;
        public DbSet<Scenario> Scenarios { get; set; } = null!;
        public DbSet<DailySummary> DailySummaries { get; set; } = null!;

        // **✅ UseSqlite: SQLite3 = 1 файл energy_iot.db**
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=EnergyManagerDB.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Таблица: User → user, DailySummary → daily_summary
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(ToSnakeCase(key.GetName()));
                }

                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()));
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
                }
            }

            // **CHECK: Type/Unit/Role** (SQL через міграції)
            modelBuilder.Entity<Device>()
                .Property(d => d.Type)
                .HasMaxLength(50);

            modelBuilder.Entity<Measurement>()
                .Property(m => m.Unit)
                .HasMaxLength(10);

            // **✅ UNIQUE: 1 запис/день/будинок** (з EnergyManagerDB.sql)
            modelBuilder.Entity<DailySummary>()
                .HasIndex(ds => new { ds.HouseId, ds.Date })
                .IsUnique();

            modelBuilder.Entity<Measurement>()
                .HasIndex(m => new { m.DeviceId, m.Timestamp })
                .IsDescending(false, true); // false для DeviceId (по возрастанию), true для Timestamp (по убыванию)


            modelBuilder.Entity<Device>()
                .HasIndex(d => d.HouseId);

            modelBuilder.Entity<Scenario>()
                .HasIndex(s => s.HouseId);

            modelBuilder.Entity<DailySummary>()
                .HasIndex(ds => new { ds.HouseId, ds.Date });

            // **FK CASCADE** (DEL house → DEL devices/measurements)
            modelBuilder.Entity<House>()
                .HasMany(h => h.Devices)
                .WithOne(d => d.House)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Device>()
                .HasMany(d => d.Measurements)
                .WithOne(m => m.Device)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<House>()
                .HasMany(h => h.Scenarios)
                .WithOne(s => s.House)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<House>()
                .HasMany(h => h.DailySummaries)
                .WithOne(ds => ds.House)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Houses)
                .WithOne(h => h.User)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            var sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0) sb.Append('_');
                    sb.Append(char.ToLower(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}
