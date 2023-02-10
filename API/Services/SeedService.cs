using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using API.Helpers;
using API.DTOs;
using API.Data;
using API.Data.Repositories;

namespace API.Services
{
    public class SeedService
    {
        private readonly UserRepository _userRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IOptions<ApiSettings> _apiSettings;
        public SeedService(UnitOfWork unitOfWork, IWebHostEnvironment env, IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings;
            _env = env;
            _unitOfWork = unitOfWork;

            _userRepository = unitOfWork.GetRepo<UserRepository>();
        }

        public async Task SeedData()
        {
            if (await CreateDatabaseAsync())
            {
                if (_env.IsDevelopment())
                    await SeedUsersAsync();


                if (await _unitOfWork.Complete())
                    return;

                throw new Exception("Database seeding operation failed");
            }
        }

        private async Task<bool> CreateDatabaseAsync()
        {
            await _unitOfWork.MigrateAsync();

            if (await _userRepository.AnyUsersAsync()) return false;

            using var hmac = new HMACSHA512();
            var admin = new AppUser
            {
                Email = _apiSettings.Value.AdminEmail,
                PasswordHash = hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(_apiSettings.Value.AdminPassword)),
                PasswordSalt = hmac.Key,
                UserRole = "Admin"
            };

            _userRepository.AddUser(admin);

            return true;
        }

        private async Task SeedUsersAsync()
        {
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var registerDtos = JsonSerializer.Deserialize<List<RegisterDto>>(userData);

            if (registerDtos == null) return;

            var defaultPassword = Encoding.UTF8.GetBytes(_apiSettings.Value.DefaultPassword);
            foreach (var r in registerDtos)
            {
                using var hmac = new HMACSHA512();
                _userRepository.AddUser(new AppUser
                {
                    Email = r.Email.ToLower(),
                    PasswordHash = hmac.ComputeHash(string.IsNullOrWhiteSpace(r.Password)
                        ? defaultPassword
                        : Encoding.UTF8.GetBytes(r.Password)),
                    PasswordSalt = hmac.Key
                });
            }
        }
    }
}