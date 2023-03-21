using System;

namespace API.DTOs
{
    public class ReservationListDto
    {
        private DateTime _startDate;
        private DateTime _endDate;
        public string ReservationName { get; set; }

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value.ToLocalTime();
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value.ToLocalTime();
        }

        public string Bike { get; set; }
    }
}