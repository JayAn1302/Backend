using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.Models;

namespace BarangayEquipmentAPI.Helpers
{
    public static class ActivityLogHelper
    {
        public static async Task LogActivity(
            ApplicationDbContext context,
            string fullName,
            string role,
            string module,
            string action)
        {
            var activity = new ActivityLog
            {
                FullName = fullName,
                Role = role,
                Module = module,
                Action = action,
                CreatedAt = DateTime.Now
            };

            context.ActivityLogs.Add(activity);

            await context.SaveChangesAsync();
        }
    }
}