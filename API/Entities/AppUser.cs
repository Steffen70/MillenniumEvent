using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string UserRole { get; set; } = "User";

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public List<Reservation> Reservations { get; set; }
    }
}