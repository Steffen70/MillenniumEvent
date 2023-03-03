using System;

namespace API.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public Guid TicketKey { get; set; } = Guid.NewGuid();
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Redeemed { get; set; } = null;
        public string Email { get; set; }

        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }

        public Ticket()
        {

        }

        public Ticket(int userId, string email)
        {
            AppUserId = userId;
            Email = email;
        }

        public override string ToString()
        {
            return TicketKey.ToString();
        }
    }
}