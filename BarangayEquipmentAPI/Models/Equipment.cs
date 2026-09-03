using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.Models
{
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        public string SerialNumber { get; set; } = string.Empty;


        [Required]
        public string EquipmentName { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int AvailableQuantity { get; set; }

        public string Condition { get; set; } = string.Empty;

        public string Status { get; set; } = "Available";

    }
}