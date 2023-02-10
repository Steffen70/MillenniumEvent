using System;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string UserRole { get; set; } = "Member";

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}