using Microsoft.EntityFrameworkCore;
using MicroCredit.Models;

namespace MicroCredit.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Account");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Loan>().ToTable("Loans");
            modelBuilder.Entity<User>()
                .Property(u => u.EncryptedPhase)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}