using System;
using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class ChangePasswordRequest
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // Other models remain the same as previous implementation
    // User, LoginRequest, CreateUserRequest, ForgotPasswordRequest, ResetPasswordRequest
}