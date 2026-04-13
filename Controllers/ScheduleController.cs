
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedTrackerScreensMVC.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly AppDbContext _db;
        public ScheduleController(AppDbContext db) { _db = db; }
        public async Task<IActionResult> Index(DateOnly? date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await SyncService.UpsertTodayFromMedications(_db, userId, HttpContext.RequestAborted);
            var target = date ?? DateOnly.FromDateTime(DateTime.Today);
            var doses = await _db.Doses.Include(d=>d.Medication).Where(d=>d.Date==target).OrderBy(d=>d.Time).ToListAsync();
            ViewData["Date"] = target; return View(doses);
        }        
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkTaken(int id) { var d = await _db.Doses.FindAsync(id); if(d==null) return NotFound(); d.Taken = true; d.TakenAt = DateTime.Now; await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index), new { date = d.Date }); }
    }
}
