using BarangayEquipmentAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BarangayEquipmentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("borrowings")]
        public IActionResult BorrowingReport()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var report = _context.Borrowings
    .Include(b => b.Borrower)
   .Include(b => b.BorrowingItems)
    .ThenInclude(i => i.Equipment)
    .ToList()
    .Select(b => new
    {
        ReferenceNo = b.ReferenceNo,

        Borrower = b.Borrower!.FullName,

        Equipment = string.Join(", ",
    b.BorrowingItems.Select(i => i.Equipment!.EquipmentName)),

        Quantity = b.BorrowingItems.Sum(i => i.QuantityBorrowed),

        BorrowDate = b.BorrowDate,

        ExpectedReturnDate = b.ExpectedReturnDate,

        Status =
            b.Status == "Borrowed" &&
            b.ExpectedReturnDate < DateOnly.FromDateTime(DateTime.Today)
                ? "Overdue"
                : b.Status
    })
    .OrderByDescending(x => x.ReferenceNo)
    .ToList();

            return Ok(report);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("equipment")]
        public IActionResult EquipmentReport()
        {
            var report = _context.Equipments

                .Select(e => new
                {
                    Equipment = e.EquipmentName,

                    Category = e.Category,

                    TotalQuantity = e.Quantity,

                    AvailableQuantity = e.AvailableQuantity,

                    BorrowedQuantity = e.Quantity - e.AvailableQuantity,

                    Status = e.Status
                })

                .OrderBy(e => e.Equipment)

                .ToList();

            return Ok(report);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("overdue")]
        public IActionResult OverdueReport()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var report = _context.Borrowings
    .AsNoTracking()
    .Include(b => b.Borrower)
    .Include(b => b.BorrowingItems)
    .ThenInclude(i => i.Equipment)
    .Where(b =>
        b.Status == "Borrowed" &&
        b.ExpectedReturnDate < today)
    .ToList()              // <-- Execute SQL first
    .Select(b => new
    {
        ReferenceNo = b.ReferenceNo,

        Borrower = b.Borrower!.FullName,

        Equipment = string.Join(", ",
    b.BorrowingItems.Select(i => i.Equipment!.EquipmentName)),

        Quantity = b.BorrowingItems.Sum(i => i.QuantityBorrowed),

        DueDate = b.ExpectedReturnDate,

        DaysOverdue = today.DayNumber - b.ExpectedReturnDate.DayNumber
    })
    .OrderBy(x => x.DueDate)
    .ToList();

            return Ok(report);
        }
    }
}