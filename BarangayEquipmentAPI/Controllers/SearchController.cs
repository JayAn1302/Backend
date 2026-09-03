using BarangayEquipmentAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarangayEquipmentAPI.DTOs.Search;

namespace BarangayEquipmentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<object>());

            var equipments = _context.Equipments
    .Where(e => e.EquipmentName.Contains(query))
                .ToList()
                .Select(e => new SearchResultDto
                {
                    Id = e.Id,
                    Title = e.EquipmentName,
                    Subtitle = "Equipment",
                    Type = "Equipment"
                });

            var borrowers = _context.Borrowers
                .Where(b => b.FullName.Contains(query))
                .ToList()
                .Select(b => new SearchResultDto
                {
                    Id = b.Id,
                    Title = b.FullName,
                    Subtitle = "Borrower",
                    Type = "Borrower"
                });

            var borrowings = _context.Borrowings
                .Include(b => b.BorrowingItems)
                    .ThenInclude(i => i.Equipment)
                .ToList()
                .Where(b => b.BorrowingItems.Any(i =>
                    i.Equipment!.EquipmentName.Contains(query)))
                .Select(b => new SearchResultDto
                {
                    Id = b.Id,
                    Title = $"BRN-{b.Id:D4}",
                    Subtitle = "Borrowing",
                    Type = "Borrowing"
                });
            var results = equipments
                .Concat<SearchResultDto>(borrowers)
                .Concat(borrowings)
                .Take(8)
                .ToList();

            return Ok(results);
        }
    }
}