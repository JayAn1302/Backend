using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarangayEquipmentAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using BarangayEquipmentAPI.Helpers;
using System.Security.Claims;

namespace BarangayEquipmentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BorrowingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Borrowings
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowingResponseDto>>> GetBorrowings()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Borrower)
                .Include(b => b.BorrowingItems)
                    .ThenInclude(i => i.Equipment)
                .ToListAsync();

            var response = borrowings.Select(b => new BorrowingResponseDto
            {
                Id = b.Id,
                ReferenceNo = b.ReferenceNo,
                BorrowerId = b.BorrowerId,
                BorrowerName = b.Borrower!.FullName,

                BorrowDate = b.BorrowDate,
                ExpectedReturnDate = b.ExpectedReturnDate,
                ActualReturnDate = b.ActualReturnDate,

                Status =
                    b.Status == "Borrowed" &&
                    b.ExpectedReturnDate < DateOnly.FromDateTime(DateTime.Today)
                        ? "Overdue"
                        : b.Status,

                Purpose = b.Purpose,
                Remarks = b.Remarks,

                Items = b.BorrowingItems.Select(i => new BorrowingItemResponseDto
                {
                    EquipmentId = i.EquipmentId,
                    EquipmentName = i.Equipment!.EquipmentName,
                    QuantityBorrowed = i.QuantityBorrowed,
                    QuantityReturned = i.QuantityReturned
                }).ToList()
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowingResponseDto>> GetBorrowing(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Borrower)
                .Include(b => b.BorrowingItems)
                    .ThenInclude(i => i.Equipment)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing == null)
                return NotFound("Borrowing record not found.");

            var response = new BorrowingResponseDto
            {
                Id = borrowing.Id,
                ReferenceNo = borrowing.ReferenceNo,

                BorrowerId = borrowing.BorrowerId,
                BorrowerName = borrowing.Borrower!.FullName,

                BorrowDate = borrowing.BorrowDate,
                ExpectedReturnDate = borrowing.ExpectedReturnDate,
                ActualReturnDate = borrowing.ActualReturnDate,

                Status =
                    borrowing.Status == "Borrowed" &&
                    borrowing.ExpectedReturnDate < DateOnly.FromDateTime(DateTime.Today)
                        ? "Overdue"
                        : borrowing.Status,

                Purpose = borrowing.Purpose,
                Remarks = borrowing.Remarks,

                Items = borrowing.BorrowingItems.Select(i => new BorrowingItemResponseDto
                {
                    EquipmentId = i.EquipmentId,
                    EquipmentName = i.Equipment!.EquipmentName,
                    QuantityBorrowed = i.QuantityBorrowed,
                    QuantityReturned = i.QuantityReturned
                }).ToList()
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<ActionResult<BorrowingResponseDto>> CreateBorrowing(CreateBorrowingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Purpose))
                return BadRequest("Purpose is required.");

            if (dto.ExpectedReturnDate < dto.BorrowDate)
                return BadRequest("Expected return date cannot be earlier than borrow date.");

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Please select at least one equipment.");

            var borrower = await _context.Borrowers.FindAsync(dto.BorrowerId);

            if (borrower == null)
                return BadRequest("Borrower not found.");

            // Validate every equipment first
            foreach (var item in dto.Items)
            {
                var equipment = await _context.Equipments.FindAsync(item.EquipmentId);

                if (equipment == null)
                    return BadRequest($"Equipment ID {item.EquipmentId} not found.");

                if (equipment.Condition == "Damaged")
                    return BadRequest($"{equipment.EquipmentName} is under maintenance.");

                if (equipment.Status != "Available")
                    return BadRequest($"{equipment.EquipmentName} is unavailable.");

                if (item.QuantityBorrowed <= 0)
                    return BadRequest("Quantity must be greater than zero.");

                if (item.QuantityBorrowed > equipment.AvailableQuantity)
                {
                    return BadRequest(
                        $"{equipment.EquipmentName} only has {equipment.AvailableQuantity} available."
                    );
                }
            }

            // ==============================
            // Generate Borrowing Reference
            // ==============================

            var lastBorrowing = await _context.Borrowings
                .OrderByDescending(b => b.Id)
                .FirstOrDefaultAsync();

            string referenceNo = lastBorrowing == null
                ? "BRN-0001"
                : $"BRN-{(lastBorrowing.Id + 1):D4}";

            // Create borrowing
            var borrowing = new Borrowing
            {
                ReferenceNo = referenceNo,
                BorrowerId = dto.BorrowerId,
                BorrowDate = dto.BorrowDate,
                ExpectedReturnDate = dto.ExpectedReturnDate,
                Purpose = dto.Purpose,
                Remarks = dto.Remarks,
                Status = "Borrowed"
            };

            _context.Borrowings.Add(borrowing);

            await _context.SaveChangesAsync();

            // Save every borrowed equipment
            foreach (var item in dto.Items)
            {
                var equipment = await _context.Equipments.FindAsync(item.EquipmentId);

                equipment!.AvailableQuantity -= item.QuantityBorrowed;

                if (equipment.AvailableQuantity == 0)
                    equipment.Status = "Unavailable";

                _context.BorrowingItems.Add(new BorrowingItem
                {
                    BorrowingId = borrowing.Id,
                    EquipmentId = item.EquipmentId,
                    QuantityBorrowed = item.QuantityBorrowed,
                    QuantityReturned = 0
                });
            }

            await _context.SaveChangesAsync();

            // Notification
            await NotificationHelper.CreateNotification(

                    _context,

                    "New Borrowing",

                    $"{borrower.FullName} borrowed {dto.Items.Count} equipment item(s).",

                    "Borrowing",

                    borrowing.Id,

                    "Borrowing"

                );


            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Borrowings",

    $"Created borrowing {borrowing.ReferenceNo} for {borrower.FullName}"

);

            var response = new BorrowingResponseDto
            {
                Id = borrowing.Id,
                ReferenceNo = borrowing.ReferenceNo,
                BorrowerId = borrower.Id,
                BorrowerName = borrower.FullName,
                BorrowDate = borrowing.BorrowDate,
                ExpectedReturnDate = borrowing.ExpectedReturnDate,
                ActualReturnDate = borrowing.ActualReturnDate,
                Status = borrowing.Status,
                Purpose = borrowing.Purpose,
                Remarks = borrowing.Remarks,

                Items = dto.Items.Select(i => new BorrowingItemResponseDto
                {
                    EquipmentId = i.EquipmentId,
                    EquipmentName = _context.Equipments
        .First(e => e.Id == i.EquipmentId)
        .EquipmentName,
                    QuantityBorrowed = i.QuantityBorrowed,
                    QuantityReturned = 0
                }).ToList()
            };

            return CreatedAtAction(nameof(GetBorrowing), new { id = borrowing.Id }, response);
        }
        [HttpPut("{id}/return")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ReturnEquipment(int id, ReturnEquipmentDto dto)
        {
            var borrowing = await _context.Borrowings
    .Include(b => b.BorrowingItems)
        .ThenInclude(i => i.Equipment)
    .Include(b => b.Borrower)
    .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing == null)
            {
                return NotFound("Borrowing not found.");
            }

            if (borrowing.Status == "Returned")
            {
                return BadRequest("Borrowing has already been returned.");
            }

            if (dto.ActualReturnDate < borrowing.BorrowDate)
            {
                return BadRequest("Actual return date cannot be earlier than borrow date.");
            }

            // Process every returned item
            foreach (var returnedItem in dto.Items)
            {
                var borrowingItem = borrowing.BorrowingItems
                    .FirstOrDefault(i => i.EquipmentId == returnedItem.EquipmentId);

                if (borrowingItem == null)
                {
                    return BadRequest($"Equipment ID {returnedItem.EquipmentId} does not belong to this borrowing.");
                }

                int remaining =
                    borrowingItem.QuantityBorrowed -
                    borrowingItem.QuantityReturned;

                if (returnedItem.QuantityReturned <= 0)
                {
                    continue;
                }

                if (returnedItem.QuantityReturned > remaining)
                {
                    return BadRequest(
                        $"Cannot return more than the remaining quantity for {borrowingItem.Equipment!.EquipmentName}."
                    );
                }

                borrowingItem.QuantityReturned += returnedItem.QuantityReturned;

                borrowingItem.Equipment!.AvailableQuantity += returnedItem.QuantityReturned;

                if (borrowingItem.Equipment.Condition != "Damaged")
                {
                    borrowingItem.Equipment.Status =
                        borrowingItem.Equipment.AvailableQuantity > 0
                            ? "Available"
                            : "Unavailable";
                }
            }

            borrowing.ActualReturnDate = dto.ActualReturnDate;
            borrowing.Remarks = dto.Remarks;

            // Determine borrowing status
            bool allReturned = borrowing.BorrowingItems.All(i =>
                i.QuantityReturned == i.QuantityBorrowed);

            bool anyReturned = borrowing.BorrowingItems.Any(i =>
                i.QuantityReturned > 0);

            if (allReturned)
            {
                borrowing.Status = "Returned";
            }
            else if (anyReturned)
            {
                borrowing.Status = "Partially Returned";
            }
            else
            {
                borrowing.Status = "Borrowed";
            }

            await _context.SaveChangesAsync();

            await NotificationHelper.CreateNotification(
                _context,
                "Equipment Returned",
                $"{borrowing.Borrower!.FullName} returned equipment from {borrowing.ReferenceNo}.",
                "Return",
                borrowing.Id,
                "Borrowing"
            );

            await ActivityLogHelper.LogActivity(
                _context,
                User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown",
                User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown",
                "Returns",
                $"Processed return for {borrowing.ReferenceNo}"
            );

            return Ok(new
            {
                message = "Return processed successfully."
            });

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowing(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.BorrowingItems)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing == null)
                return NotFound("Borrowing record not found.");

            if (borrowing.Status == "Borrowed")
                return BadRequest("Cannot delete an active borrowing record.");

            _context.BorrowingItems.RemoveRange(borrowing.BorrowingItems);

            _context.Borrowings.Remove(borrowing);

            await _context.SaveChangesAsync();


            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Borrowings",

    $"Deleted borrowing {borrowing.ReferenceNo}"

);
            return Ok(new
            {
                message = "Borrowing deleted successfully."
            });
        }
    }
}