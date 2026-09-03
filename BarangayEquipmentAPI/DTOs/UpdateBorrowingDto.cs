namespace BarangayEquipmentAPI.DTOs
{
    public class UpdateBorrowingDto
    {
        public int BorrowerId { get; set; }

        public int EquipmentId { get; set; }

        public int QuantityBorrowed { get; set; }

        public DateOnly BorrowDate { get; set; }

        public DateOnly ExpectedReturnDate { get; set; }

        public string? Purpose { get; set; }

        public string? Remarks { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}