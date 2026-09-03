namespace BarangayEquipmentAPI.DTOs
{
    public class BorrowingResponseDto
    {
        public int Id { get; set; }

        public string ReferenceNo { get; set; } = string.Empty;

        public int BorrowerId { get; set; }

        public string BorrowerName { get; set; } = string.Empty;

        public DateOnly BorrowDate { get; set; }

        public DateOnly ExpectedReturnDate { get; set; }

        public DateOnly? ActualReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Purpose { get; set; }

        public string? Remarks { get; set; }

        public List<BorrowingItemResponseDto> Items { get; set; } = new();
    }

    public class BorrowingItemResponseDto
    {
        public int EquipmentId { get; set; }

        public string EquipmentName { get; set; } = string.Empty;

        public int QuantityBorrowed { get; set; }

        public int QuantityReturned { get; set; }
    }
}