using API.Entities;
using System;

namespace API.DTOs
{
    public class ReservationCreateDto
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value.ToUniversalTime();
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value.ToUniversalTime();
        }

        public int AppUserId { get; set; }
        public int BikeId { get; set; }
    }
}