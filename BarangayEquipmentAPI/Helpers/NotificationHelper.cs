using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.Models;

namespace BarangayEquipmentAPI.Helpers
{
    public static class NotificationHelper
    {
        public static async Task CreateNotification(
            ApplicationDbContext context,
            string title,
            string message,
            string type,
            int? referenceId = null,
            string? referenceType = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.Now,
                ReferenceId = referenceId,
                ReferenceType = referenceType
            };

            context.Notifications.Add(notification);

            await context.SaveChangesAsync();
        }
    }
}