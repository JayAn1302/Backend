using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarangayEquipmentAPI.Models
{
    public class Borrowing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReferenceNo { get; set; } = string.Empty;

        [Required]
        public int BorrowerId { get; set; }

        [ForeignKey(nameof(BorrowerId))]
        public Borrower? Borrower { get; set; }

        [Required]
        public DateOnly BorrowDate { get; set; }

        [Required]
        public DateOnly ExpectedReturnDate { get; set; }

        public DateOnly? ActualReturnDate { get; set; }

        public string Status { get; set; } = "Borrowed";

        public string? Purpose { get; set; }

        public string? Remarks { get; set; }

        [Required]

        // One Borrowing -> Many BorrowingItems
        public ICollection<BorrowingItem> BorrowingItems { get; set; }
            = new List<BorrowingItem>();
    }
}