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

        [HttpPost("login")]
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
    }
}