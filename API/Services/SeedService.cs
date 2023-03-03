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
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class SeedService
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IOptions<ApiSettings> _apiSettings;
        public SeedService(DataContext context, IWebHostEnvironment env, IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings;
            _env = env;
            _context = context;
        }

        public async Task SeedData()
        {
            if (await CreateDatabaseAsync())
            {
                if (_env.IsDevelopment())
                    await SeedUsersAsync();


                if (await _context.SaveChangesAsync() > 0)
                    return;

                throw new Exception("Database seeding operation failed");
            }
        }

        private async Task<bool> CreateDatabaseAsync()
        {
            await _context.Database.MigrateAsync();

            if (await _context.Users.AnyAsync()) return false;

            using var hmac = new HMACSHA512();
            var admin = new AppUser
            {
                Email = _apiSettings.Value.AdminEmail,
                PasswordHash = Convert.ToBase64String(hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(_apiSettings.Value.AdminPassword))),
                PasswordSalt = Convert.ToBase64String(hmac.Key),
                UserRole = "Admin"
            };

            _context.Users.Add(admin);

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
                _context.Users.Add(new AppUser
                {
                    Email = r.Email.ToLower(),
                    PasswordHash = Convert.ToBase64String(hmac.ComputeHash(string.IsNullOrWhiteSpace(r.Password) ? defaultPassword : Encoding.UTF8.GetBytes(r.Password))),
                    PasswordSalt = Convert.ToBase64String(hmac.Key),
                    UserRole = "Promoter"
                });
            }
        }
    }
}