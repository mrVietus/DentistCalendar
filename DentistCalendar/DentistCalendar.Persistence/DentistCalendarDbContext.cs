using DentistCalendar.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentistCalendar.Persistence
{
    public class DentistCalendarDbContext : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Dentist> Dentists { get; set; }
        public DbSet<DentistOffice> DentistOffices { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<MenuElement> MenuElements { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }

        public DentistCalendarDbContext(DbContextOptions<DentistCalendarDbContext> options)
           : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Dentist - DentistOffice - many to many
            modelBuilder.Entity<DentistDentistOffice>()
                .HasKey(pc => new { pc.DentistId, pc.DentistOfficeId });

            modelBuilder.Entity<DentistDentistOffice>()
                .HasOne(pc => pc.Dentist)
                .WithMany(p => p.DentistDentistOffices)
                .HasForeignKey(pc => pc.DentistId);


            modelBuilder.Entity<DentistDentistOffice>()
                .HasOne(pc => pc.DentistOffice)
                .WithMany(c => c.DentistDentistOffices)
                .HasForeignKey(pc => pc.DentistOfficeId);

            //Dentist - Service - many to many
            modelBuilder.Entity<DentistService>()
                .HasKey(pc => new { pc.DentistId, pc.ServiceId });

            modelBuilder.Entity<DentistService>()
                .HasOne(pc => pc.Dentist)
                .WithMany(p => p.DentistServices)
                .HasForeignKey(pc => pc.DentistId);

            modelBuilder.Entity<DentistService>()
                .HasOne(pc => pc.Service)
                .WithMany(c => c.DentistServices)
                .HasForeignKey(pc => pc.ServiceId);
        }
    }
}