using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string UserRole { get; set; } = "Member";

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public List<Ticket> Tickets { get; set; }
    }
}