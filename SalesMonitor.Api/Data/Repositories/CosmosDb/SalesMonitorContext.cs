using Microsoft.EntityFrameworkCore;

namespace SalesMonitor.Api.Data
{
    public class SalesMonitorContext : DbContext
    {
        public DbSet<DboSalesEntry> SalesEntries { get; set; }

        public SalesMonitorContext(DbContextOptions<SalesMonitorContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Store");

            modelBuilder.Entity<DboSalesEntry>()
                .ToContainer("SalesEntries");

            modelBuilder.Entity<DboSalesEntry>()
                .HasNoDiscriminator();
        }
    }
}