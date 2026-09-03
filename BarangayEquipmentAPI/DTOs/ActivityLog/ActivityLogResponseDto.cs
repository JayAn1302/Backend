namespace BarangayEquipmentAPI.DTOs.ActivityLog
{
    public class ActivityLogResponseDto
    {
        public int ActivityLogId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Module { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}