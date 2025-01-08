using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OptiSched.Models;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Availabilitie> Availabilities { get; set; }
    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacja między AppEvent a User1 (pierwszy użytkownik)
        modelBuilder.Entity<Meeting>()
            .HasOne(e => e.User1)
            .WithMany()
            .HasForeignKey(e => e.UserId1)
            .OnDelete(DeleteBehavior.Restrict); // Zachowanie przy usuwaniu, np. Restrict lub Cascade

        // Relacja między AppEvent a User2 (drugi użytkownik)
        modelBuilder.Entity<Meeting>()
            .HasOne(e => e.User2)
            .WithMany()
            .HasForeignKey(e => e.UserId2)
            .OnDelete(DeleteBehavior.Restrict); // Zachowanie przy usuwaniu, np. Restrict lub Cascade

        // Relacja jeden do jeden: Meeting -> Schedule
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Meeting)
            .WithOne(m => m.Schedule)
            .HasForeignKey<Schedule>(s => s.MeetingId); // MeetingId w Schedule jest kluczem obcym

        // Relacja jeden do wielu: AppUser -> Accessibility
        modelBuilder.Entity<Availabilitie>()
            .HasOne(a => a.AppUser)
            .WithMany(u => u.Availabilities)
            .HasForeignKey(a => a.AppUserId)
            .OnDelete(DeleteBehavior.Cascade); // Przy usuwaniu AppUser, usuwamy też Accessibility

        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Room)
            .WithOne(r => r.Schedule)
            .HasForeignKey<Schedule>(s => s.RoomId);

        // Ustawienie długości kolumn
        modelBuilder.Entity<AppUser>()
            .Property(u => u.Name)
            .HasMaxLength(150)
            .IsRequired(false); // Jeśli Name może być null

        modelBuilder.Entity<AppUser>()
            .Property(u => u.Surname)
            .HasMaxLength(150)
            .IsRequired(false); // Jeśli Surname może być null
    }
}