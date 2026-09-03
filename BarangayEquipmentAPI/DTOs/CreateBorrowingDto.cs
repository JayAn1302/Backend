using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.DTOs
{
    public class CreateBorrowingDto
    {
        [Required]
        public int BorrowerId { get; set; }

        [Required]
        public DateOnly BorrowDate { get; set; }

        [Required]
        public DateOnly ExpectedReturnDate { get; set; }

        public string? Purpose { get; set; }

        public string? Remarks { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one equipment item is required.")]
        public List<CreateBorrowingItemDto> Items { get; set; } = new();
    }
}