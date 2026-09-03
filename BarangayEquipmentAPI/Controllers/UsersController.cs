using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.DTOs;
using BarangayEquipmentAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarangayEquipmentAPI.Helpers;
using System.Security.Claims;

namespace BarangayEquipmentAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Username = u.Username,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found.");

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive
            });
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("Username already exists.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                IsActive = true
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            await NotificationHelper.CreateNotification(

                _context,

                "New User",

                $"{user.FullName} was added as a {user.Role}.",

                "User",

                user.Id,

                "User"

            );

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Users",

    $"Created user: {user.FullName}"

);

            return Ok(new
            {
                message = "User created successfully."
            });

        }

            // PUT: api/Users/5
            [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found.");

            if (await _context.Users.AnyAsync(u =>
                    u.Username == dto.Username &&
                    u.Id != id))
            {
                return BadRequest("Username already exists.");
            }

            user.FullName = dto.FullName;
            user.Username = dto.Username;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Users",

    $"Updated user: {user.FullName}"

);

            return Ok(new
            {
                message = "User updated successfully."
            });
        }

        // PATCH: api/Users/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found.");

            if (user.Username == User.Identity!.Name)
            {
                return BadRequest("You cannot deactivate your own account.");
            }

            user.IsActive = !user.IsActive;

            await _context.SaveChangesAsync();

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Users",

    user.IsActive
        ? $"Activated user: {user.FullName}"
        : $"Deactivated user: {user.FullName}"

);

            return Ok(new
            {
                message = user.IsActive
                    ? "User activated successfully."
                    : "User deactivated successfully."
            });
        }
    }
}