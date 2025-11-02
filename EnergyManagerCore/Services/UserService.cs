using EnergyManagerCore.Models;
using EnergyManagerCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Services
{
    // Models/DTOs/UserDtos.cs
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";  // Hash в Service
        public string Role { get; set; } = "user";
    }

    // Services/IUserService.cs
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto?> UpdateAsync(int id, UserDto dto);
        Task<bool> DeleteAsync(int id);
    }

    // Services/UserService.cs
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IAuthRepository _authRepo;  // Для Hash

        public UserService(IUserRepository repo, IAuthRepository authRepo)
        {
            _repo = repo;
            _authRepo = authRepo;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(u => new UserDto { Id = u.Id, Username = u.Username, Email = u.Email, Role = u.Role });
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user != null ? new UserDto { Id = user.Id, Username = user.Username, Email = user.Email, Role = user.Role } : null;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };
            var created = await _repo.CreateAsync(user);
            return new UserDto { Id = created.Id, Username = created.Username, Email = created.Email, Role = created.Role };
        }

        public async Task<UserDto?> UpdateAsync(int id, UserDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            existing.Username = dto.Username;
            existing.Email = dto.Email;
            existing.Role = dto.Role;
            // Пароль не изменяется

            var updated = await _repo.UpdateAsync(id, existing);
            return new UserDto
            {
                Id = updated.Id,
                Username = updated.Username,
                Email = updated.Email,
                Role = updated.Role
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            return await _repo.DeleteAsync(id);
        }

    }
}
