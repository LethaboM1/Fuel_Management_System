using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FMS.Data;
using FMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class UsersController : ControllerBase
    {
        private readonly FMSDbContext _context;

        public UsersController(FMSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.EmployeeNumber,
                    u.Role,
                    u.Station,
                    u.CreatedAt,
                    u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.EmployeeNumber,
                    u.Role,
                    u.Station,
                    u.CreatedAt,
                    u.IsActive
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }

            // Check if employee number is being changed and if it already exists
            if (user.EmployeeNumber != request.EmployeeNumber)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmployeeNumber == request.EmployeeNumber);
                
                if (existingUser != null)
                {
                    return BadRequest("Employee number already exists");
                }
            }

            // Update user properties
            user.FullName = request.FullName;
            user.EmployeeNumber = request.EmployeeNumber;
            
            if (!string.IsNullOrEmpty(request.Role))
            {
                user.Role = request.Role;
            }
            
            if (!string.IsNullOrEmpty(request.Station))
            {
                user.Station = request.Station;
            }
            
            if (request.IsActive.HasValue)
            {
                user.IsActive = request.IsActive.Value;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User updated successfully",
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.EmployeeNumber,
                    user.Role,
                    user.Station,
                    user.IsActive
                }
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }

            // Soft delete - mark as inactive instead of removing
            user.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deactivated successfully" });
        }

        [HttpGet("profile")]
        public async Task<ActionResult<object>> GetProfile()
        {
            // Get current user from JWT token
            var employeeNumber = User.FindFirst("EmployeeNumber")?.Value;
            
            if (string.IsNullOrEmpty(employeeNumber))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.EmployeeNumber == employeeNumber && u.IsActive)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.EmployeeNumber,
                    u.Role,
                    u.Station,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            // Get current user from JWT token
            var employeeNumber = User.FindFirst("EmployeeNumber")?.Value;
            
            if (string.IsNullOrEmpty(employeeNumber))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmployeeNumber == employeeNumber && u.IsActive);

            if (user == null)
            {
                return NotFound();
            }

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Update allowed fields (users can only update their name and station, not role or employee number)
            if (!string.IsNullOrEmpty(request.FullName))
            {
                user.FullName = request.FullName;
            }
            
            if (!string.IsNullOrEmpty(request.Station))
            {
                user.Station = request.Station;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully",
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.EmployeeNumber,
                    user.Role,
                    user.Station,
                    user.CreatedAt
                }
            });
        }
    }

    // Add this model for profile updates (users can't change their own role or employee number)
    public class UpdateProfileRequest
    {
        [MaxLength(100)]
        public string? FullName { get; set; }
        
        [MaxLength(100)]
        public string? Station { get; set; }
    }
}