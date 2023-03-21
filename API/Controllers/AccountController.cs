using System;
using System.Collections.Generic;
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
            var loginRoles = new[] { "Admin", "Employee" };
            var user = await Context.Users
                .SingleOrDefaultAsync(x => loginRoles.Any(r => r == x.UserRole) &&  x.Username == loginDto.Username.ToLower());

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

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpPost("Create")]
        public ActionResult/*<string>*/ Create([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username) ||  username.Contains(' '))
                return BadRequest();

            if (Context.Users.Any(u => u.Username == username))
                return StatusCode(208);

            Context.Users.Add(new AppUser
            {
                Username = username.ToLower(),
                UserRole = "User"
            });

            Context.SaveChanges();

            return Ok();
        }

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<UserListDto>>> List()
        {
            var list = await Context.Users.OrderBy(u => u.UserRole).ToListAsync();
            var userListDtoList = Mapper.Map<IEnumerable<UserListDto>>(list);
            return Ok(userListDtoList);
        }
    }
}