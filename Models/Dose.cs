
using System.ComponentModel.DataAnnotations;

namespace MedTrackerScreensMVC.Models
{
    public class Dose
    {
        public int Id { get; set; }
        public string UserId { get; set; } =string.Empty;   
        public int MedicationId { get; set; }
        public Medication? Medication { get; set; }
        public DateOnly Date { get; set; }
        [StringLength(5)] public string Time { get; set; } = "08:00";
        public bool Taken { get; set; }
        public DateTime? TakenAt { get; set; }
    }
}
