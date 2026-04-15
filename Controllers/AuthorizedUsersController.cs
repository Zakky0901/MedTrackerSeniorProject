using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MedTrackerScreensMVC.Controllers
{
    [Authorize]
    public class AuthorizedUsersController : Controller
    {
        private readonly AppDbContext _db;
        public AuthorizedUsersController(AppDbContext db) { _db = db; }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            return View(await _db.AuthorizedUsers
                .Include(a => a.RelationshipType)
                .OrderBy(a => a.FullName)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Relationships = _db.RelationshipTypes.OrderBy(r => r.Name).ToList();
            return View(new AuthorizedUser());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorizedUser u)
        {
            var userId = GetUserId();
            u.UserId = userId!;
            ModelState.Remove(nameof(AuthorizedUser.UserId));

            if (!ModelState.IsValid)
            {
                ViewBag.Relationships = _db.RelationshipTypes.OrderBy(r => r.Name).ToList();
                return View(u);
            }
            _db.Add(u);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            var u = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (u == null) return NotFound();
            ViewBag.Relationships = _db.RelationshipTypes.OrderBy(r => r.Name).ToList();
            return View(u);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorizedUser u)
        {
            if (id != u.Id) return BadRequest();
            var userId = GetUserId();
            u.UserId = userId!;
            ModelState.Remove(nameof(AuthorizedUser.UserId));

            if (!ModelState.IsValid)
            {
                ViewBag.Relationships = _db.RelationshipTypes.OrderBy(r => r.Name).ToList();
                return View(u);
            }
            _db.Update(u);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var u = await _db.AuthorizedUsers.Include(a => a.RelationshipType)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            var u = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (u != null) { _db.AuthorizedUsers.Remove(u); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}