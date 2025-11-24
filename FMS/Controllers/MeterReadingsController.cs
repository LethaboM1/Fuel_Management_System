using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FMS.Data;
using FMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeterReadingsController : ControllerBase
    {
        private readonly FMSDbContext _context;

        public MeterReadingsController(FMSDbContext context)
        {
            _context = context;
        }

        // GET: api/meterreadings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeterReading>>> GetMeterReadings(
            [FromQuery] string plantNumber = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.MeterReadings.AsQueryable();

            if (!string.IsNullOrEmpty(plantNumber))
                query = query.Where(m => m.PlantNumber == plantNumber);

            if (startDate.HasValue)
                query = query.Where(m => m.ReadingDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.ReadingDate <= endDate.Value);

            var readings = await query
                .OrderByDescending(m => m.ReadingDate)
                .ThenBy(m => m.PlantNumber)
                .ToListAsync();

            return Ok(readings);
        }

        // GET: api/meterreadings/plant/FSH01-01/latest
        [HttpGet("plant/{plantNumber}/latest")]
        public async Task<ActionResult<MeterReading>> GetLatestMeterReading(string plantNumber)
        {
            var latestReading = await _context.MeterReadings
                .Where(m => m.PlantNumber == plantNumber)
                .OrderByDescending(m => m.ReadingDate)
                .FirstOrDefaultAsync();

            if (latestReading == null)
            {
                return NotFound($"No meter readings found for plant {plantNumber}");
            }

            return latestReading;
        }

        // GET: api/meterreadings/plant/FSH01-01/history
        [HttpGet("plant/{plantNumber}/history")]
        public async Task<ActionResult<IEnumerable<MeterReading>>> GetMeterReadingHistory(string plantNumber)
        {
            var readings = await _context.MeterReadings
                .Where(m => m.PlantNumber == plantNumber)
                .OrderByDescending(m => m.ReadingDate)
                .Take(50) // Last 50 readings
                .ToListAsync();

            return Ok(readings);
        }

        // POST: api/meterreadings
        [HttpPost]
        public async Task<ActionResult<MeterReading>> PostMeterReading(MeterReading meterReading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate reading value is positive
            if (meterReading.ReadingValue < 0)
            {
                return BadRequest("Meter reading value must be positive");
            }

            meterReading.CreatedAt = DateTime.UtcNow;
            _context.MeterReadings.Add(meterReading);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMeterReadings), new { id = meterReading.Id }, meterReading);
        }

        // GET: api/meterreadings/plant/FSH01-01/consumption
        [HttpGet("plant/{plantNumber}/consumption")]
        public async Task<ActionResult<object>> GetConsumptionReport(string plantNumber, [FromQuery] int days = 30)
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-days);

            var readings = await _context.MeterReadings
                .Where(m => m.PlantNumber == plantNumber && m.ReadingDate >= startDate && m.ReadingDate <= endDate)
                .OrderBy(m => m.ReadingDate)
                .ToListAsync();

            if (readings.Count < 2)
            {
                return Ok(new { 
                    plantNumber,
                    period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                    totalConsumption = 0m,
                    averageDailyConsumption = 0m,
                    message = "Insufficient data for consumption calculation"
                });
            }

            var totalConsumption = readings.Last().ReadingValue - readings.First().ReadingValue;
            var averageDailyConsumption = totalConsumption / days;

            return Ok(new
            {
                plantNumber,
                period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                totalConsumption,
                averageDailyConsumption,
                readingCount = readings.Count,
                firstReading = readings.First().ReadingValue,
                lastReading = readings.Last().ReadingValue
            });
        }
    }
}