using Microsoft.AspNetCore.Mvc;
using FMS.Data;
using Microsoft.AspNetCore.Authorization;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly FMSDbContext _context;

        public TestController(FMSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API is working!" });
        }

        [HttpGet("db-test")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                // Test if database can be connected to
                var canConnect = await _context.Database.CanConnectAsync();
                return Ok(new { 
                    databaseConnected = canConnect,
                    message = canConnect ? "Database connection successful" : "Database connection failed"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    databaseConnected = false,
                    message = "Database connection failed",
                    error = ex.Message
                });
            }
        }
    }
}