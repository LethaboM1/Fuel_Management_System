using System;
using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    // User entity for database
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string EmployeeNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "attendant";
        
        [Required]
        [MaxLength(100)]
        public string Station { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        [MaxLength(100)]
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }

    // Login request model
    public class LoginRequest
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string EmployeeNumber { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    // Create user request model
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string EmployeeNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Station { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    // Update user request model - ADD THIS
    public class UpdateUserRequest
    {
        [Required]
        public string EmployeeNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        public string? Role { get; set; }
        
        public string? Station { get; set; }
        
        public bool? IsActive { get; set; }
    }

    // Forgot password request model
    public class ForgotPasswordRequest
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string EmployeeNumber { get; set; } = string.Empty;
    }

    // Reset password request model
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // Change password request model
    public class ChangePasswordRequest
    {
        [Required]
        public string EmployeeNumber { get; set; } = string.Empty;
        
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}