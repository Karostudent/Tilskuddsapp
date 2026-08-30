using Microsoft.EntityFrameworkCore;
using Tilskuddsapp.Models;

namespace Tilskuddsapp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<GrantCase> GrantCases { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurer relasjoner
            modelBuilder.Entity<GrantCase>()
                .HasOne(g => g.Doctor)
                .WithMany(d => d.GrantCases)
                .HasForeignKey(g => g.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GrantCase>()
                .HasOne(g => g.Supervisor)
                .WithMany(s => s.GrantCases)
                .HasForeignKey(g => g.SupervisorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Konfigurer decimal presisjon for pengebeløp
            modelBuilder.Entity<GrantCase>()
                .Property(g => g.ApprovedGrant)
                .HasPrecision(18, 2);

            modelBuilder.Entity<GrantCase>()
                .Property(g => g.MaximumGrant)
                .HasPrecision(18, 2);

            modelBuilder.Entity<GrantCase>()
                .Property(g => g.SupervisionHours)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Supervisor>()
                .Property(s => s.HourlyRate)
                .HasPrecision(18, 2);
        }
    }
}
