using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.DTOs
{
    public class UpdateBorrowerDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string ContactNumber { get; set; } = string.Empty;

        // NEW
        public IFormFile? Photo { get; set; }
    }
}