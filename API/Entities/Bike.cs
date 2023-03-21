using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace API.Entities
{
    public enum Category
    {
        [Description("A: Unbeschränkt")]
        A,
        [Description("-A: bis 35kW")]
        ABeschraenkt,
        [Description("A1: bis 11kW")]
        A1
    }

    public class Bike
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public Category Category { get; set; }

        public List<Reservation> Reservations { get; set; }

        public override string ToString()
        {
            return $"{Year} {Brand} {Model}";
        }
    }
}
