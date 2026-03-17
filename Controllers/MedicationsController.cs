
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Models;
using MedTrackerScreensMVC.Services;

namespace MedTrackerScreensMVC.Controllers
{
    public class MedicationsController : Controller
    {
        private readonly AppDbContext _db;
        public MedicationsController(AppDbContext db) { _db = db; }
        public async Task<IActionResult> Index()
        {
            await SyncService.UpsertTodayFromMedications(_db, HttpContext.RequestAborted);
            var meds = await _db.Medications.OrderBy(m => m.Name).ToListAsync();
            var today = DateOnly.FromDateTime(DateTime.Today);
            var status = await _db.Doses.Where(d => d.Date == today)
                .GroupBy(d => d.MedicationId)
                .Select(g => new { MedId = g.Key, Taken = g.Any(x=>x.Taken), Exists = g.Any() })
                .ToListAsync();
            ViewBag.TodayStatus = status.ToDictionary(x=>x.MedId, x=> x.Taken ? "Taken" : "Due");
            return View(meds);
        }
        public IActionResult Create() => View(new Medication());
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medication m) { if(!ModelState.IsValid) return View(m); _db.Add(m); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Edit(int id) { var m = await _db.Medications.FindAsync(id); if(m==null) return NotFound(); return View(m); }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medication m) { if(id!=m.Id) return BadRequest(); if(!ModelState.IsValid) return View(m); _db.Update(m); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Delete(int id) { var m = await _db.Medications.FindAsync(id); if(m==null) return NotFound(); return View(m); }
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var m = await _db.Medications.FindAsync(id); if(m!=null){ _db.Medications.Remove(m); await _db.SaveChangesAsync(); } return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Details(int id) { var m = await _db.Medications.FindAsync(id); if(m==null) return NotFound(); return View(m); }
    }
}
