using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FMS.Data;
using FMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LedgerCodesController : ControllerBase
    {
        private readonly FMSDbContext _context;

        public LedgerCodesController(FMSDbContext context)
        {
            _context = context;
        }

        // GET: api/ledgercodes
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LedgerCode>>> GetLedgerCodes(
            [FromQuery] string category = null,
            [FromQuery] string fuelType = null,
            [FromQuery] bool activeOnly = true)
        {
            var query = _context.LedgerCodes.AsQueryable();

            if (activeOnly)
                query = query.Where(l => l.IsActive);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(l => l.Category == category);

            if (!string.IsNullOrEmpty(fuelType))
                query = query.Where(l => l.FuelType == fuelType);

            var ledgerCodes = await query
                .OrderBy(l => l.Category)
                .ThenBy(l => l.Code)
                .ToListAsync();

            return Ok(ledgerCodes);
        }

        // GET: api/ledgercodes/categories
        [HttpGet("categories")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<string>>> GetLedgerCodeCategories()
        {
            var categories = await _context.LedgerCodes
                .Where(l => l.IsActive)
                .Select(l => l.Category)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/ledgercodes/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<LedgerCode>> GetLedgerCode(int id)
        {
            var ledgerCode = await _context.LedgerCodes.FindAsync(id);

            if (ledgerCode == null)
            {
                return NotFound();
            }

            return ledgerCode;
        }

        // POST: api/ledgercodes
        [HttpPost]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<LedgerCode>> PostLedgerCode(LedgerCode ledgerCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if code already exists
            if (await _context.LedgerCodes.AnyAsync(l => l.Code == ledgerCode.Code))
            {
                return BadRequest("Ledger code already exists");
            }

            ledgerCode.CreatedAt = DateTime.UtcNow;
            _context.LedgerCodes.Add(ledgerCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLedgerCode), new { id = ledgerCode.Id }, ledgerCode);
        }

        // PUT: api/ledgercodes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> PutLedgerCode(int id, LedgerCode ledgerCode)
        {
            if (id != ledgerCode.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if code already exists (excluding current record)
            if (await _context.LedgerCodes.AnyAsync(l => l.Code == ledgerCode.Code && l.Id != id))
            {
                return BadRequest("Ledger code already exists");
            }

            _context.Entry(ledgerCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LedgerCodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ledgercodes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> DeleteLedgerCode(int id)
        {
            var ledgerCode = await _context.LedgerCodes.FindAsync(id);
            if (ledgerCode == null)
            {
                return NotFound();
            }

            // Soft delete - mark as inactive
            ledgerCode.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LedgerCodeExists(int id)
        {
            return _context.LedgerCodes.Any(e => e.Id == id);
        }
    }
}