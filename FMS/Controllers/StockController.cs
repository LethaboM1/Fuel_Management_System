using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FMS.Data;
using FMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly FMSDbContext _context;

        public StockController(FMSDbContext context)
        {
            _context = context;
        }

        // GET: api/stock
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStock()
        {
            var stocks = await _context.Stocks
                .OrderBy(s => s.Category)
                .ThenBy(s => s.PlantName)
                .ToListAsync();

            return Ok(stocks);
        }

        // GET: api/stock/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStockByCategory(string category)
        {
            var stocks = await _context.Stocks
                .Where(s => s.Category == category)
                .ToListAsync();

            return Ok(stocks);
        }

        // PUT: api/stock/5
        [HttpPut("{plantId}")]
        public async Task<IActionResult> UpdateStock(string plantId, [FromBody] UpdateStockRequest request)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.PlantId == plantId);

            if (stock == null)
            {
                return NotFound();
            }

            stock.CurrentStock = request.NewStock;
            stock.LastUpdated = DateTime.UtcNow;
            stock.LastUpdatedBy = request.UpdatedBy;

            await _context.SaveChangesAsync();

            return Ok(stock);
        }

        // GET: api/stock/alerts
        [HttpGet("alerts")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetLowStockAlerts()
        {
            var alertThreshold = 0.2m; // 20% threshold

            var lowStocks = await _context.Stocks
                .Where(s => s.CurrentStock / s.Capacity <= alertThreshold)
                .ToListAsync();

            return Ok(lowStocks);
        }
    }

    public class UpdateStockRequest
    {
        public decimal NewStock { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}