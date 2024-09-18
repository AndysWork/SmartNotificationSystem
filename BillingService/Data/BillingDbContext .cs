using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Data
{
    public class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
    {
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>()
                .Property(o => o.TotalCart)
                .HasComputedColumnSql("[Quantity]* [Price]");
        }
    }
}
