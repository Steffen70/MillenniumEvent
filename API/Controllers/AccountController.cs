using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Data.Repositories;
using API.DTOs;
using API.Entities;
using API.Services;
using AutoMapper;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseApiController
    {
        private readonly TokenService _tokenService;
        private readonly UserRepository _userRepository;
        public AccountController(UnitOfWork unitOfWork, IMapper mapper, TokenService tokenService) : base(unitOfWork, mapper)
        {
            _userRepository = unitOfWork.GetRepo<UserRepository>();
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userRepository.UserExistsAsync(registerDto.Email))
                return BadRequest("Email is taken");

            using var hmac = new HMACSHA512();
            var user = _mapper.Map<AppUser>(registerDto);

            user.Email = registerDto.Email.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _userRepository.AddUser(user);

            if (!await _unitOfWork.Complete())
                throw new Exception("Registration failed, the user could not be created");

            var userDto = new UserDto { Token = _tokenService.CreateToken(user) };
            return _mapper.Map(user, userDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user is null) return Unauthorized("Invalid Email");

            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                    if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            var userDto = new UserDto { Token = _tokenService.CreateToken(user) };
            return _mapper.Map(user, userDto);
        }
    }
}