// using System;
// using System.ComponentModel.DataAnnotations;

// namespace FMS.Models
// {
//     public class User
//     {
//         [Key]
//         public int Id { get; set; }
        
//         [Required]
//         public string FullName { get; set; } = string.Empty;
        
//         [Required]
//         [EmailAddress]
//         public string Email { get; set; } = string.Empty;
        
//         [Required]
//         public string Role { get; set; } = "attendant"; // attendant, admin, manager
        
//         [Required]
//         public string Station { get; set; } = string.Empty;
        
//         [Required]
//         public string PasswordHash { get; set; } = string.Empty;
        
//         public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//         public bool IsActive { get; set; } = true;
//     }

//     public class LoginRequest
//     {
//         [Required]
//         [EmailAddress]
//         public string Email { get; set; } = string.Empty;
        
//         [Required]
//         public string Password { get; set; } = string.Empty;
//     }

//     public class CreateUserRequest
//     {
//         [Required]
//         public string FullName { get; set; } = string.Empty;
        
//         [Required]
//         [EmailAddress]
//         public string Email { get; set; } = string.Empty;
        
//         [Required]
//         public string Role { get; set; } = string.Empty;
        
//         [Required]
//         public string Station { get; set; } = string.Empty;
        
//         [Required]
//         public string Password { get; set; } = string.Empty;
//     }

// }

using System;
using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "attendant";
        
        [Required]
        public string Station { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class CreateUserRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = string.Empty;
        
        [Required]
        public string Station { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}