using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class LedgerCode
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string Category { get; set; } = string.Empty; // Fuel, Maintenance, Sundries, etc.
        
        public string FuelType { get; set; } = string.Empty; // Diesel, Petrol, etc.
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}