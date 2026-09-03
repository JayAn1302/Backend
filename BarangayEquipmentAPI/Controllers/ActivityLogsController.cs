using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.DTOs.ActivityLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarangayEquipmentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityLogResponseDto>>> GetActivityLogs()
        {
            var logs = await _context.ActivityLogs
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ActivityLogResponseDto
                {
                    ActivityLogId = a.ActivityLogId,
                    FullName = a.FullName,
                    Role = a.Role,
                    Module = a.Module,
                    Action = a.Action,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}