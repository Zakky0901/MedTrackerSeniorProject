
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await SyncService.UpsertTodayFromMedications(_db, userId, HttpContext.RequestAborted);
            var target = date ?? DateOnly.FromDateTime(DateTime.Today);
            var doses = await _db.Doses.Include(d=>d.Medication).Where(d=>d.Date==target).Where(d=>d.UserId == userId).OrderBy(d=>d.Time).ToListAsync();
            ViewData["Date"] = target; return View(doses);
        }        
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkTaken(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); var d = await _db.Doses
    .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);if(d==null) return NotFound(); d.Taken = true; d.TakenAt = DateTime.UtcNow; await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index), new { date = d.Date } ); }
    }
}
