using Microsoft.EntityFrameworkCore;
using DocumentProcessing.Domain.Models;

namespace DocumentProcessing.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasIndex(d => d.FileHash)
                .IsUnique();
        }
    }
}
