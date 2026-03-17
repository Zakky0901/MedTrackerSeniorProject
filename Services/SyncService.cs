
using MedTrackerScreensMVC.Data;
using MedTrackerScreensMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MedTrackerScreensMVC.Services
{
    public static class SyncService
    {
        public static async Task UpsertTodayFromMedications(AppDbContext db, CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var meds = await db.Medications.Where(m => m.IsActive).ToListAsync(ct);
            foreach (var m in meds)
            {
                if (string.IsNullOrWhiteSpace(m.TimeOfDay)) continue;
                bool exists = await db.Doses.AnyAsync(d => d.MedicationId == m.Id && d.Date == today && d.Time == m.TimeOfDay!, ct);
                if (!exists)
                {
                    db.Doses.Add(new Dose
                    {
                        MedicationId = m.Id,
                        Date = today,
                        Time = m.TimeOfDay!,
                        Taken = false
                    });
                }
            }
            await db.SaveChangesAsync(ct);
        }
    }
}
