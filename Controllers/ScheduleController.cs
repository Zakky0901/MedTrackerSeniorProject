using MedTrackerScreensMVC.Data;
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

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index(DateOnly? date)
        {
            var userId = GetUserId();
            var target = date ?? DateOnly.FromDateTime(DateTime.Today);
            var doses = await _db.Doses
                .Include(d => d.Medication)
                .Where(d => d.Date == target && d.Medication!.UserId == userId)
                .OrderBy(d => d.Time)
                .ToListAsync();
            ViewData["Date"] = target;
            return View(doses);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkTaken(int id)
        {
            var userId = GetUserId();
            var d = await _db.Doses.Include(d => d.Medication)
                .FirstOrDefaultAsync(d => d.Id == id && d.Medication!.UserId == userId);
            if (d == null) return NotFound();
            d.Taken = true;
            d.TakenAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { date = d.Date });
        }
    }
}