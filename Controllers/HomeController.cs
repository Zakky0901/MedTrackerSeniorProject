using Auth0.AspNetCore.Authentication;
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedTrackerScreensMVC.Controllers
{
    [Authorize] // Redirects unauthenticated users to login
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Login", "Account");
            
            await SyncService.UpsertTodayFromMedications(_db, userId, HttpContext.RequestAborted);

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Filter all queries by the current user's ID
            ViewData["ActiveMedications"] = _db.Medications.Count(m => m.UserId == userId && m.IsActive);
            ViewData["PlannedToday"] = _db.Doses.Count(d => d.Date == today && d.Medication!.UserId == userId);
            ViewData["TakenToday"] = _db.Doses.Count(d => d.Date == today && d.Taken && d.Medication!.UserId == userId);

            var todayDoses = await _db.Doses
               .Include(d => d.Medication)
               .Where(d => d.Date == today && d.Medication!.UserId == userId)
               .OrderBy(d => d.Time)
               .ToListAsync();

            return View(todayDoses);
        }
    }
}