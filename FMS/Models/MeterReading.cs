using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class MeterReading
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string PlantNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime ReadingDate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal ReadingValue { get; set; }
        
        [Required]
        public string Unit { get; set; } = string.Empty; // Liters, Kilometers, Hours
        
        public string ReadingType { get; set; } = "Regular"; // Regular, Initial, Adjustment
        
        public string TakenBy { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public string VehicleId { get; set; } = string.Empty;
    }
}