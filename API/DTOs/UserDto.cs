using System;

namespace API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string UserRole { get; set; }
        public string Token { get; set; }
    }
}