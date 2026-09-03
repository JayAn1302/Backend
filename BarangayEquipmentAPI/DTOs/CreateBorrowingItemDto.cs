using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.DTOs
{
    public class CreateBorrowingItemDto
    {
        [Required]
        public int EquipmentId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int QuantityBorrowed { get; set; }
    }
}