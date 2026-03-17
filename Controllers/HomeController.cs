
using Microsoft.AspNetCore.Mvc;
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Services;

namespace MedTrackerScreensMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }
        public async Task<IActionResult> Index()
        {
            await SyncService.UpsertTodayFromMedications(_db, HttpContext.RequestAborted);
            var today = DateOnly.FromDateTime(DateTime.Today);
            ViewData["ActiveMedications"] = _db.Medications.Count(m => m.IsActive);
            ViewData["PlannedToday"] = _db.Doses.Count(d => d.Date == today);
            ViewData["TakenToday"] = _db.Doses.Count(d => d.Date == today && d.Taken);
            return View();
        }
    }
}
