using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.Models
{
    public class Borrower
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string ContactNumber { get; set; } = string.Empty;

        // NEW
        public string? PhotoPath { get; set; }
    }
}