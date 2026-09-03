using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.DTOs
{
    public class UpdateEquipmentDto
    {
        [Required]
        public string EquipmentName { get; set; } = string.Empty;

        public string SerialNumber { get; set; } = string.Empty;


        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]

        public string Condition { get; set; } = string.Empty;

        public string Status { get; set; } = "Available";

    }    
}