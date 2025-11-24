using System;
using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class Transactions
    {
        [Key]
        public int Id { get; set; }
        
        // Fuel Store Selection
        [Required]
        public string FuelStoreCategory { get; set; } = string.Empty;
        
        [Required]
        public string PlantNumber { get; set; } = string.Empty;
        
        // Date
        [Required]
        public DateTime TransactionDate { get; set; }
        
        // Meter Readings
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Previous meter reading must be positive")]
        public decimal PreviousMeterReading { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Current meter reading must be positive")]
        public decimal CurrentMeterReading { get; set; }
        
        [Required]
        public string MeterUnit { get; set; } = string.Empty;
        
        // Calculated Consumption
        public decimal Consumption => CurrentMeterReading - PreviousMeterReading;
        
        // Contract Details
        public string Contract { get; set; } = string.Empty;
        public string CustomContract { get; set; } = string.Empty;
        
        // Store Transaction Details
        public string VatType { get; set; } = "Inclusive";
        public string Store { get; set; } = string.Empty;
        public string StockItem { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal InStock { get; set; }
        public decimal Rate { get; set; }
        public string StockUnit { get; set; } = string.Empty;
        
        // Currency and Quantities
        public string Currency { get; set; } = "ZAR";
        public string IssueUnit { get; set; } = string.Empty;
        public decimal ConvertBy { get; set; } = 1;
        public decimal Quantity { get; set; }
        public decimal ConvertQuantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public decimal Vat { get; set; }
        
        // Ledger Code
        [Required]
        public string LedgerCode { get; set; } = string.Empty;
        
        // Additional fields
        public string AttendantId { get; set; } = string.Empty;
        public string AttendantName { get; set; } = string.Empty;
        public string Station { get; set; } = string.Empty;
        public string Status { get; set; } = "completed";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Validation method
        public bool IsValidMeterReading()
        {
            return CurrentMeterReading > PreviousMeterReading;
        }
        
        public string GetMeterReadingValidationMessage()
        {
            if (CurrentMeterReading <= PreviousMeterReading)
                return "Current meter reading must be greater than previous reading";
            return "Valid";
        }
    }
}