using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string PlantId { get; set; } = string.Empty;
        
        [Required]
        public string PlantName { get; set; } = string.Empty;
        
        [Required]
        public string Category { get; set; } = string.Empty;
        
        public decimal CurrentStock { get; set; }
        public decimal Capacity { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public string LastUpdatedBy { get; set; } = "System";
    }
}