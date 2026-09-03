using System.ComponentModel.DataAnnotations;

namespace BarangayEquipmentAPI.DTOs.Notification
{
    public class CreateNotificationDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;
    }
}