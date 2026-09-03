using BarangayEquipmentAPI.Data;
using BarangayEquipmentAPI.DTOs;
using BarangayEquipmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using BarangayEquipmentAPI.Helpers;
using System.Security.Claims;


namespace BarangayEquipmentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BorrowersController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowerResponseDto>>> GetBorrowers()
        {
            var borrowers = await _context.Borrowers
                .Select(b => new BorrowerResponseDto
                {
                    Id = b.Id,
                    FullName = b.FullName,
                    Address = b.Address,
                    ContactNumber = b.ContactNumber,
                    PhotoPath = b.PhotoPath
                })
                .ToListAsync();

            return Ok(borrowers);
        }


        // GET: api/Borrowers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowerResponseDto>> GetBorrower(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);

            if (borrower == null)
                return NotFound();

            return new BorrowerResponseDto
            {
                Id = borrower.Id,
                FullName = borrower.FullName,
                Address = borrower.Address,
                ContactNumber = borrower.ContactNumber,
                PhotoPath = borrower.PhotoPath
            };
        }
        // POST: api/Borrowers
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BorrowerResponseDto>> CreateBorrower([FromForm] CreateBorrowerDto dto)
        {
            string? photoPath = null;

            if (dto.Photo != null)
            {
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "borrowers"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(dto.Photo.FileName);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Photo.CopyToAsync(stream);
                }

                photoPath = $"uploads/borrowers/{fileName}";
            }

            var borrower = new Borrower
            {
                FullName = dto.FullName,
                Address = dto.Address,
                ContactNumber = dto.ContactNumber,
                PhotoPath = photoPath
            };

            _context.Borrowers.Add(borrower);

            await _context.SaveChangesAsync();

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Borrowers",

    $"Added borrower: {borrower.FullName}"

);

            await NotificationHelper.CreateNotification(

                _context,

                "New Borrower",

                $"{borrower.FullName} was added as a new borrower.",

                "Borrower",

                borrower.Id,

                "Borrower"

            );

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Borrowers",

    $"Added borrower: {borrower.FullName}"

);

            var response = new BorrowerResponseDto
            {
                Id = borrower.Id,
                FullName = borrower.FullName,
                Address = borrower.Address,
                ContactNumber = borrower.ContactNumber,
                PhotoPath = borrower.PhotoPath
            };

            return CreatedAtAction(
                nameof(GetBorrower),
                new { id = borrower.Id },
                response
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBorrower(
    int id,
    [FromForm] UpdateBorrowerDto dto)
        {
            var borrower = await _context.Borrowers.FindAsync(id);

            if (borrower == null)
                return NotFound();

            borrower.FullName = dto.FullName;
            borrower.Address = dto.Address;
            borrower.ContactNumber = dto.ContactNumber;

            if (dto.Photo != null)
            {
                // Delete old photo
                if (!string.IsNullOrEmpty(borrower.PhotoPath))
                {
                    var oldFile = Path.Combine(
                        _environment.WebRootPath,
                        borrower.PhotoPath.Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "borrowers"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(dto.Photo.FileName);

                var filePath = Path.Combine(
                    uploadsFolder,
                    fileName
                );

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Photo.CopyToAsync(stream);
                }

                borrower.PhotoPath = $"uploads/borrowers/{fileName}";
            }

            await _context.SaveChangesAsync();

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Borrowers",

    $"Updated borrower: {borrower.FullName}"

);

            return NoContent();
        }

        // DELETE: api/Borrowers/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrower(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);

            if (borrower == null)
            {
                return NotFound("Borrower not found.");
            }

            // Check if borrower has an active borrowing
            bool hasActiveBorrowing = await _context.Borrowings
                .AnyAsync(b => b.BorrowerId == id && b.Status == "Borrowed");

            if (hasActiveBorrowing)
            {
                return BadRequest(
                    "Cannot delete borrower because they have an active borrowing."
                );
            }

            if (!string.IsNullOrEmpty(borrower.PhotoPath))
            {
                var file = Path.Combine(
                    _environment.WebRootPath,
                    borrower.PhotoPath.Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            _context.Borrowers.Remove(borrower);

            await _context.SaveChangesAsync();

            await ActivityLogHelper.LogActivity(

    _context,

    User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",

    User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown",

    "Borrowers",

    $"Deleted borrower: {borrower.FullName}"

);

            return Ok(new
            {
                message = "Borrower deleted successfully."
            });
        }
    }
}