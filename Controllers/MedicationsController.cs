using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Models;
using MedTrackerScreensMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedTrackerScreensMVC.Controllers
{
    [Authorize] // 🔒 Protect all actions
    public class MedicationsController : Controller
    {
        private readonly AppDbContext _db;
        public MedicationsController(AppDbContext db) { _db = db; }

        // Helper to get current userId cleanly
        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

           // await SyncService.UpsertTodayFromMedications(_db, userId, HttpContext.RequestAborted);

            // Filter medications by current user
            var meds = await _db.Medications
                .OrderBy(m => m.Name)
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Filter today's doses by current user via Medication
            var status = await _db.Doses
                .Where(d => d.Date == today && d.Medication!.UserId == userId)
                .GroupBy(d => d.MedicationId)
                .Select(g => new { MedId = g.Key, Taken = g.Any(x => x.Taken) })
                .ToListAsync();

            ViewBag.TodayStatus = status.ToDictionary(x => x.MedId, x => x.Taken ? "Taken" : "Due");

            return View(meds);
        }

        public IActionResult Create() => View(new Medication());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medication m)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            // Stamp the current user's ID before saving
            m.UserId = userId;

            // Remove UserId from ModelState validation since we set it manually
            ModelState.Remove(nameof(Medication.UserId));

            if (!ModelState.IsValid) return View(m);

            _db.Add(m);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            // Ensure user can only edit their own medications
            var m = await _db.Medications.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (m == null) return NotFound();
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medication m)
        {
            if (id != m.Id) return BadRequest();

            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            // Preserve the UserId — never trust it from the form
            m.UserId = userId;
            ModelState.Remove(nameof(Medication.UserId));

            if (!ModelState.IsValid) return View(m);

            _db.Update(m);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            // Ensure user can only delete their own medications
            var m = await _db.Medications.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (m == null) return NotFound();
            return View(m);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var m = await _db.Medications.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (m != null)
            {
                _db.Medications.Remove(m);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            // Ensure user can only view their own medications
            var m = await _db.Medications.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (m == null) return NotFound();
            return View(m);
        }
    }
}