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

    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Models.Type> Types { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Room> Rooms { get; set; }
}
