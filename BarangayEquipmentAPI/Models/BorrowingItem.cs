using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarangayEquipmentAPI.Models
{
    public class BorrowingItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BorrowingId { get; set; }

        [ForeignKey(nameof(BorrowingId))]
        public Borrowing? Borrowing { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        [ForeignKey(nameof(EquipmentId))]
        public Equipment? Equipment { get; set; }

        [Required]
        public int QuantityBorrowed { get; set; }

        public int QuantityReturned { get; set; } = 0;
    }
}