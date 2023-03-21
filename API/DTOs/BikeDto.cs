using API.Entities;
using System;

namespace API.DTOs
{
    public class BikeDto
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Category { get; set; }
    }
}