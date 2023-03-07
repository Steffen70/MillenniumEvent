using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.DTOs;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using Org.BouncyCastle.Crypto.Macs;
using SkiaSharp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseApiController
    {
        private readonly TokenService _tokenService;
        public AccountController(DataContext context, IMapper mapper, TokenService tokenService) : base(context, mapper)
        {
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await Context.Users
                .SingleOrDefaultAsync(x => x.Email == loginDto.Email.ToLower());

            if (user is null) return Unauthorized("Invalid Email");

            using (var hmac = new HMACSHA512(Convert.FromBase64String(user.PasswordSalt)))
            {
                var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)));

                if (!user.PasswordHash.Equals(computedHash))
                    return Unauthorized("Invalid password");
            }

            var userDto = new UserDto { Token = _tokenService.CreateToken(user) };
            return Mapper.Map(user, userDto);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("Create")]
        public ActionResult<string> Create([FromQuery] string email)
        {
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!regex.Match(email).Success)
                return BadRequest();

            if (Context.Users.Any(u => u.Email == email))
                return StatusCode(208);

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "C# program");
            var response = client.GetStringAsync($"https://www.dinopass.com/password/simple").ConfigureAwait(true);

            var password = response.GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(password)) return StatusCode(418);

            using var hmac = new HMACSHA512();
            Context.Users.Add(new AppUser
            {
                Email = email.ToLower(),
                PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password))),
                PasswordSalt = Convert.ToBase64String(hmac.Key),
                UserRole = "Promoter"
            });

            Context.SaveChanges();
            return password;
        }
    }
}