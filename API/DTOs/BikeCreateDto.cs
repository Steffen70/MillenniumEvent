using API.Entities;
using System;

namespace API.DTOs
{
    public class BikeCreateDto
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Category { get; set; }
    }
}