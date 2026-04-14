
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedTrackerScreensMVC.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly AppDbContext _db;
        public ScheduleController(AppDbContext db) { _db = db; }
        public async Task<IActionResult> Index(DateOnly? date)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Content("ERROR: userId is NULL");
                }

                var target = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

                var doses = await _db.Doses
                    .Include(d => d.UserId == userId)
                    .ToListAsync();

                return Content($"SUCCESS: Found {doses.Count} doses for user {userId}");
            }
            catch (Exception ex)
            {
                return Content("CRASH: " + ex.ToString());
            }
        }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkTaken(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); var d = await _db.Doses
    .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);if(d==null) return NotFound(); d.Taken = true; d.TakenAt = DateTime.UtcNow; await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index), new { date = d.Date } ); }
    }
}
