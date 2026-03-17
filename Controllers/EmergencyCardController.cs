
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Models;

namespace MedTrackerScreensMVC.Controllers
{
    public class EmergencyCardController : Controller
    {
        private readonly AppDbContext _db;
        public EmergencyCardController(AppDbContext db) { _db = db; }
        public async Task<IActionResult> Index() { ViewBag.BloodTypes = await _db.BloodTypes.OrderBy(b=>b.Name).ToListAsync(); var card = await _db.EmergencyCards.Include(c=>c.BloodType).FirstOrDefaultAsync() ?? new EmergencyCard(); return View(card); }        
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EmergencyCard card) { if(!ModelState.IsValid) { ViewBag.BloodTypes = await _db.BloodTypes.OrderBy(b=>b.Name).ToListAsync(); return View(card); } if(card.Id==0) _db.EmergencyCards.Add(card); else _db.EmergencyCards.Update(card); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Print() { var card = await _db.EmergencyCards.Include(c=>c.BloodType).FirstOrDefaultAsync() ?? new EmergencyCard(); return View(card); }        
    }
}
