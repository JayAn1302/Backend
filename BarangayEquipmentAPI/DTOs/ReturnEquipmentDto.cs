namespace BarangayEquipmentAPI.DTOs
{
    public class ReturnEquipmentDto
    {
        public DateOnly ActualReturnDate { get; set; }

        public string? Remarks { get; set; }

        public List<ReturnEquipmentItemDto> Items { get; set; } = new();
    }
}