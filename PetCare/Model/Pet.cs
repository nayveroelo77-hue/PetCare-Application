using SQLite;

namespace PetCare.Model
{
    public class Pet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Species { get; set; } = string.Empty;

        public string Breed { get; set; } = string.Empty;

        public int Age { get; set; }

        public double Weight { get; set; }

        public int OwnerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Ignore]
        public string? LatestNotes { get; set; }

        [Ignore]
        public bool HasAdminNotes => !string.IsNullOrWhiteSpace(LatestNotes);
    }
}
