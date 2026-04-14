
using Microsoft.EntityFrameworkCore;
using MedTrackerScreensMVC.Models;

namespace MedTrackerScreensMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Medication> Medications => Set<Medication>();
        public DbSet<Dose> Doses => Set<Dose>();
        public DbSet<AuthorizedUser> AuthorizedUsers => Set<AuthorizedUser>();
        public DbSet<RelationshipType> RelationshipTypes => Set<RelationshipType>();
        public DbSet<BloodType> BloodTypes => Set<BloodType>();
        public DbSet<EmergencyCard> EmergencyCards => Set<EmergencyCard>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Dose>()
                .Property(d => d.UserId)
                .HasColumnName("UserId"); 
            modelBuilder.Entity<Dose>()
                .HasIndex(d => new { d.MedicationId, d.Date, d.Time})  
                .IsUnique();
            modelBuilder.Entity<AuthorizedUser>()
                .HasOne(a => a.RelationshipType)
                .WithMany()
                .HasForeignKey(a => a.RelationshipTypeId);
            modelBuilder.Entity<EmergencyCard>()
                .HasOne(e => e.BloodType)
                .WithMany()
                .HasForeignKey(e => e.BloodTypeId);
        }
    }
}
