using BarangayEquipmentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BarangayEquipmentAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<Borrower> Borrowers { get; set; }

        public DbSet<Borrowing> Borrowings { get; set; }

        public DbSet<BorrowingItem> BorrowingItems { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }


    }
}