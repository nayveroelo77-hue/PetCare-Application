namespace PetCare.Model
{
    public class CareGuide
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., Grooming, Nutrition, Health
        public string ShortDescription { get; set; } = string.Empty;
        public string FullContent { get; set; } = string.Empty;
        public string Icon { get; set; } = "paw.png"; // Default icon
        
        // Category styling mappings
        public string CategoryColor => Category switch
        {
            "Grooming" => "#1565C0",
            "Nutrition" => "#2E7D32",
            "Health" => "#D32F2F",
            "Training" => "#F57C00",
            _ => "#FF8200"
        };
        
        public string CategoryBgColor => Category switch
        {
            "Grooming" => "#E3F2FD",
            "Nutrition" => "#E8F5E9",
            "Health" => "#FFEBEE",
            "Training" => "#FFF3E0",
            _ => "#FFF5E6"
        };
    }
}
