using System.ComponentModel.DataAnnotations;
namespace MedTrackerScreensMVC.Models
{
    public class Medication
    {
        public int Id { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        [StringLength(50)] public string? DosageDisplay { get; set; }
        [StringLength(20)] public string? Frequency { get; set; }
        [StringLength(5)] public string? TimeOfDay { get; set; }
        public int PillsRemaining { get; set; }
        public DateOnly? RefillDate { get; set; }
        [StringLength(1000)] public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Dose> Doses { get; set; } = new List<Dose>();
    }
}
