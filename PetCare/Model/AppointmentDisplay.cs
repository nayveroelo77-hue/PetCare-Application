using PetCare.Model;
using Microsoft.Maui.Graphics;

namespace PetCare.Model
{
    public class AppointmentDisplay
    {
        public Appointment Appointment { get; set; }
        public string PetName { get; set; } = "Unknown";
        public string OwnerName { get; set; } = "Unknown";

        // Status visual indicators
        public Color StatusColor => Appointment.Status switch
        {
            "Approved" => Color.FromArgb("#2E7D32"),
            "Completed" => Color.FromArgb("#1565C0"),
            "Rejected" or "Cancelled" => Color.FromArgb("#C62828"),
            "Scheduled" or "Pending" => Color.FromArgb("#FF8200"),
            _ => Color.FromArgb("#757575")
        };

        public string StatusLabel => Appointment.Status.ToUpper();

        public string StatusIcon => Appointment.Status switch
        {
            "Completed" => "✅",
            "Approved" => "📋",
            "Rejected" or "Cancelled" => "❌",
            "Scheduled" or "Pending" => "⏳",
            _ => "📅"
        };

        public bool HasNotes => !string.IsNullOrWhiteSpace(Appointment.Notes);

        // Visibility Logic for UI Actions
        public bool IsPending => Appointment.Status == "Scheduled" || Appointment.Status == "Pending";
        public bool IsApproved => Appointment.Status == "Approved";
        public bool IsRejected => Appointment.Status == "Rejected";
        public bool IsCompleted => Appointment.Status == "Completed";
        public bool IsCancellable => Appointment.Status == "Scheduled" || Appointment.Status == "Pending" || Appointment.Status == "Approved";
        
        // Finalized states cannot be modified or deleted
        public bool IsNotFinalized => !IsApproved && !IsCompleted;
    }
}
