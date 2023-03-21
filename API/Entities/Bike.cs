using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace API.Entities
{
    public enum Category
    {
        [Description("Kategorie A: Unbeschränkt")]
        A,
        [Description("Kategorie -A: bis 35kW")]
        ABeschraenkt,
        [Description("Kategorie A1: bis 11kW")]
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
    }
}
