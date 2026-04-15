
using System.ComponentModel.DataAnnotations;

namespace MedTrackerScreensMVC.Models
{
    public class AuthorizedUser
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        [Required, StringLength(120)] public string FullName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a relationship type.")]
        public int RelationshipTypeId { get; set; }
        public RelationshipType? RelationshipType { get; set; }
        [StringLength(120)] public string? Email { get; set; }
        [StringLength(40)] public string? Phone { get; set; }
        public DateOnly AddedOn { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
