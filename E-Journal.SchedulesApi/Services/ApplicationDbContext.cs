using Microsoft.EntityFrameworkCore;
using E_Journal.SchedulesApi.Models;

namespace E_Journal.SchedulesApi.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SchedulesApi.Models.Lesson>().HasKey(m => new { m.GroupId, m.Date, m.Number, m.Subgroup });
        modelBuilder.Entity<Group>().HasAlternateKey(m => m.Name);
        modelBuilder.Entity<Subject>().HasAlternateKey(m => m.Name);
        modelBuilder.Entity<Models.Type>().HasAlternateKey(m => m.Name);
        modelBuilder.Entity<Teacher>().HasAlternateKey(m => m.Name);
        modelBuilder.Entity<Room>().HasAlternateKey(m => m.Name);
    }

    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Models.Type> Types { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Room> Rooms { get; set; }
}
