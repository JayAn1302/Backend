using BarangayEquipmentAPI.DTOs;
using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BarangayEquipmentAPI.Helpers;
using System.Security.Claims;

namespace BarangayEquipmentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Equipment
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentResponseDto>>> GetEquipments()
        {
            var equipments = await _context.Equipments
                .Select(e => new EquipmentResponseDto
                {
                    Id = e.Id,
                    SerialNumber = e.SerialNumber,
                    EquipmentName = e.EquipmentName,
                    Category = e.Category,
                    Quantity = e.Quantity,
                    AvailableQuantity = e.AvailableQuantity,
                    Condition = e.Condition,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(equipments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentResponseDto>> GetEquipment(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound("Equipment not found.");
            }

            var response = new EquipmentResponseDto
            {
                Id = equipment.Id,
                SerialNumber = equipment.SerialNumber,
                EquipmentName = equipment.EquipmentName,
                Category = equipment.Category,
                Quantity = equipment.Quantity,
                AvailableQuantity = equipment.AvailableQuantity,
                Condition = equipment.Condition,
                Status = equipment.Status
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EquipmentResponseDto>> CreateEquipment(CreateEquipmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EquipmentName))
                return BadRequest("Equipment name is required.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                return BadRequest("Category is required.");

            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            bool equipmentExists = await _context.Equipments.AnyAsync(e =>
    e.EquipmentName.ToLower() == dto.EquipmentName.ToLower());

            if (equipmentExists)
            {
                return BadRequest("An equipment with this name already exists.");
            }

            // Get the latest equipment
            var lastEquipment = await _context.Equipments
                .OrderByDescending(e => e.Id)
                .FirstOrDefaultAsync();

            // Generate Serial Number
            string serialNumber = lastEquipment == null
                ? "EQ-0001"
                : $"EQ-{(lastEquipment.Id + 1):D4}";


            var equipment = new Equipment
            {
                SerialNumber = serialNumber,
                EquipmentName = dto.EquipmentName,
                Category = dto.Category,
                Quantity = dto.Quantity,

                // Automatically set Available Quantity
                AvailableQuantity = dto.Quantity,

                Condition = dto.Condition,

                Status = dto.Condition == "Damaged"
                    ? "Under Maintenance"
                    : "Available",
            };

            _context.Equipments.Add(equipment);

            await _context.SaveChangesAsync();

            await NotificationHelper.CreateNotification(

                _context,

                "New Equipment",

                $"{equipment.EquipmentName} was added to the inventory.",

                "Equipment",

                equipment.Id,

                "Equipment"

            );

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Equipment",

    $"Added equipment: {equipment.EquipmentName}"

);

            return Ok(equipment);
        }

        // PUT: api/Equipment/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipment(int id, UpdateEquipmentDto dto)
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
                return NotFound("Equipment not found.");

            // Validation
            if (string.IsNullOrWhiteSpace(dto.EquipmentName))
                return BadRequest("Equipment name is required.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                return BadRequest("Category is required.");

            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            bool duplicateName = await _context.Equipments.AnyAsync(e =>
    e.Id != id &&
    e.EquipmentName.ToLower() == dto.EquipmentName.ToLower());

            if (duplicateName)
            {
                return BadRequest("Another equipment already uses this name.");
            }


            equipment.EquipmentName = dto.EquipmentName;
            equipment.Category = dto.Category;

            // Compute how many are currently borrowed
            int borrowed = equipment.Quantity - equipment.AvailableQuantity;

            // Update information
            equipment.EquipmentName = dto.EquipmentName;
            equipment.Category = dto.Category;
            equipment.Quantity = dto.Quantity;

            // Recalculate available quantity
            equipment.AvailableQuantity = dto.Quantity - borrowed;

            if (equipment.AvailableQuantity < 0)
            {
                equipment.AvailableQuantity = 0;
            }

            equipment.Condition = dto.Condition;

            equipment.Status = equipment.AvailableQuantity > 0
                ? "Available"
                : "Unavailable";


            if (equipment.Condition == "Damaged")
            {
                equipment.Status = "Under Maintenance";
            }
            else
            {
                equipment.Status =
                    equipment.AvailableQuantity > 0
                        ? "Available"
                        : "Unavailable";
            }

            await _context.SaveChangesAsync();

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Equipment",

    $"Updated equipment: {equipment.EquipmentName}"

);

            return Ok(new
            {
                message = "Equipment updated successfully."
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound("Equipment not found.");
            }

            // Check if equipment has active borrowings
            bool hasActiveBorrowing = await _context.BorrowingItems
    .Include(i => i.Borrowing)
    .AnyAsync(i =>
        i.EquipmentId == id &&
        i.Borrowing!.Status == "Borrowed");

            if (hasActiveBorrowing)
            {
                return BadRequest("Cannot delete equipment because it is currently borrowed.");
            }

            _context.Equipments.Remove(equipment);

            await _context.SaveChangesAsync();

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Equipment",

    $"Deleted equipment: {equipment.EquipmentName}"

);

            return Ok(new
            {
                message = "Equipment deleted successfully."
            });

        }
        [HttpGet("search")]
        public async Task<ActionResult> SearchEquipment(string keyword)
        {
            var equipments = await _context.Equipments

                .Where(e =>

    e.EquipmentName.Contains(keyword) ||

    e.SerialNumber.Contains(keyword) ||

    e.Category.Contains(keyword)

)

                .ToListAsync();

            return Ok(equipments);
        }
    }
}