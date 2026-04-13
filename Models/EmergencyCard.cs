
using System.ComponentModel.DataAnnotations;

namespace MedTrackerScreensMVC.Models
{
    public class EmergencyCard
    {
        public int Id { get; set; }
        [Display(Name = "Date of Birth")] public DateOnly? DateOfBirth { get; set; }
        [Display(Name = "Last 4 Digits of SSN"), StringLength(4)] public string? Last4SSN { get; set; }
        public int? BloodTypeId { get; set; }
        public BloodType? BloodType { get; set; }
        [StringLength(120)] public string? InsuranceProvider { get; set; }
        [StringLength(80)] public string? InsurancePolicyNumber { get; set; }
    }
}
