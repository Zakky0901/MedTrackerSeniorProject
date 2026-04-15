
using System.ComponentModel.DataAnnotations;

namespace MedTrackerScreensMVC.Models
{
    public class RelationshipType
    {
        public int Id { get; set; }
        [Required, StringLength(60)] public string Name { get; set; } = string.Empty;

    }
}
