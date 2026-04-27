using SQLite;

namespace PetCare.Model
{
    [Table("UserAccount")]
    public class UserAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        [Unique]
        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        // "Admin" or "Client"
        public string Role { get; set; } = "Client";

        [Ignore]
        public bool IsAdmin => Role == "Admin";

        [Ignore]
        public bool IsNotAdmin => !IsAdmin;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
