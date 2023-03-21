using System;

namespace API.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ReservationName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Bike Bike { get; set; }
        public int BikeId { get; set; }
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }

    }
}
