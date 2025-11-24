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
    public class LedgerCodesController : ControllerBase
    {
        private readonly FMSDbContext _context;

        public LedgerCodesController(FMSDbContext context)
        {
            _context = context;
        }

        // GET: api/ledgercodes
        [HttpGet]
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
    }
}