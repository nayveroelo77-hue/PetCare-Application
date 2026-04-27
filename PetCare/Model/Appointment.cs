using SQLite;
using System;

namespace PetCare.Model
{
    public class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int PetId { get; set; }

        public DateTime DateTime { get; set; }

        public string ServiceType { get; set; } = string.Empty; // Checkup, Grooming, Vaccination, etc.

        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled

        public string? Notes { get; set; }
    }
}
