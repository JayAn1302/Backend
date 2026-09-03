namespace BarangayEquipmentAPI.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? ReferenceId { get; set; }

        public string? ReferenceType { get; set; }
    }
}