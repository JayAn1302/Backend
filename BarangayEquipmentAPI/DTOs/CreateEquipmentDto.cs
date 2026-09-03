using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.DTOs
{
    public class CreateEquipmentDto
    {
        [Required]
        public string EquipmentName { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        public string Condition { get; set; } = string.Empty;


    }
}