using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarangayEquipmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardResponseDto>> GetDashboard()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var response = new DashboardResponseDto
            {
                // Equipment Cards
                TotalEquipment = await _context.Equipments.CountAsync(),

                AvailableEquipment = await _context.Equipments
                    .SumAsync(e => e.AvailableQuantity),
                BorrowedEquipment = await _context.BorrowingItems
                .SumAsync(i => i.QuantityBorrowed),

                MaintenanceEquipment = await _context.Equipments
                    .CountAsync(e => e.Status == "Maintenance"),

                ReservedEquipment = await _context.Equipments
                    .CountAsync(e => e.Status == "Reserved"),

                // Borrowers
                TotalBorrowers = await _context.Borrowers.CountAsync(),

                // Borrowings
                ActiveBorrowings = await _context.Borrowings
                    .CountAsync(b => b.Status == "Borrowed"),

                ReturnedBorrowings = await _context.Borrowings
                    .CountAsync(b => b.Status == "Returned"),

                OverdueBorrowings = await _context.Borrowings
                    .CountAsync(b =>
                        b.Status == "Borrowed" &&
                        b.ExpectedReturnDate < today),

                RecentBorrowings = await _context.Borrowings
                .Include(b => b.Borrower)
                .Include(b => b.BorrowingItems)
                    .ThenInclude(i => i.Equipment)
                .OrderByDescending(b => b.Id)
                .Take(6)
                .Select(b => new BorrowingResponseDto
                {
                    Id = b.Id,

                    BorrowerId = b.BorrowerId,
                    BorrowerName = b.Borrower!.FullName,

                    BorrowDate = b.BorrowDate,
                    ExpectedReturnDate = b.ExpectedReturnDate,
                    ActualReturnDate = b.ActualReturnDate,

                    Status =
                        b.Status == "Borrowed" &&
                        b.ExpectedReturnDate < today
                            ? "Overdue"
                            : b.Status,

                    Purpose = b.Purpose,
                    Remarks = b.Remarks,

                    Items = b.BorrowingItems.Select(i => new BorrowingItemResponseDto
                    {
                        EquipmentId = i.EquipmentId,
                        EquipmentName = i.Equipment!.EquipmentName,
                        QuantityBorrowed = i.QuantityBorrowed
                    }).ToList()
                })
                .ToListAsync()
                        };

            return Ok(response);
        }
    }
  
}