
using System.ComponentModel.DataAnnotations;

namespace MedTrackerScreensMVC.Models
{
    public class BloodType
    {
        public int Id { get; set; }
        [Required, StringLength(5)] public string Name { get; set; } = string.Empty;
    }
}
