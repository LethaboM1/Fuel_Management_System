// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using FMS.Data;
// using FMS.Models;
// using Microsoft.Extensions.Configuration;

// namespace FMS.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class AuthController : ControllerBase
//     {
//         private readonly FMSDbContext _context;
//         private readonly IConfiguration _configuration;

//         public AuthController(FMSDbContext context, IConfiguration configuration)
//         {
//             _context = context;
//             _configuration = configuration;
//         }

//         [HttpPost("login")]
//         public async Task<ActionResult<object>> Login(LoginRequest request)
//         {
//             var user = await _context.Users
//                 .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

//             if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//             {
//                 return Unauthorized("Invalid email or password");
//             }

//             var token = GenerateJwtToken(user);

//             return Ok(new
//             {
//                 Token = token,
//                 User = new
//                 {
//                     user.Id,
//                     user.FullName,
//                     user.Email,
//                     user.Role,
//                     user.Station
//                 }
//             });
//         }

//         private string GenerateJwtToken(User user)
//         {
//             var claims = new[]
//             {
//                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                 new Claim(ClaimTypes.Name, user.FullName),
//                 new Claim(ClaimTypes.Email, user.Email),
//                 new Claim(ClaimTypes.Role, user.Role),
//                 new Claim("Station", user.Station)
//             };

//             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
//                 _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
//             var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//             var expires = DateTime.Now.AddDays(7);

//             var token = new JwtSecurityToken(
//                 _configuration["Jwt:Issuer"] ?? "FMS_API",
//                 _configuration["Jwt:Audience"] ?? "FMS_Client",
//                 claims,
//                 expires: expires,
//                 signingCredentials: creds
//             );

//             return new JwtSecurityTokenHandler().WriteToken(token);
//         }
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FMS.Data;
using FMS.Models;
using FMS.Services;
using Microsoft.Extensions.Configuration;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly FMSDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthController(FMSDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Role,
                    user.Station
                }
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register(CreateUserRequest request)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
                Station = request.Station,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Role,
                    user.Station
                }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return Ok(new { message = "If the email exists, a password reset link has been sent." });
            }

            // Generate reset token
            var resetToken = Guid.NewGuid().ToString();
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            
            await _context.SaveChangesAsync();

            try
            {
                // Send email via SMTP
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetToken);
                return Ok(new { message = "Password reset instructions have been sent to your email." });
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return StatusCode(500, new { message = "Failed to send email. Please try again later." });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ResetToken == request.Token && 
                                         u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest("Invalid or expired reset token");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully" });
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest("Current password is incorrect");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Station", user.Station ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"] ?? "FMS_API",
                _configuration["Jwt:Audience"] ?? "FMS_Client",
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}