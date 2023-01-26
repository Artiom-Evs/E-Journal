using Microsoft.EntityFrameworkCore;
using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        Database.Migrate();
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<TrainingType> TrainingTypes { get; set; }
    public DbSet<Training> Trainings { get; set; }
    public DbSet<MarkValue> MarkValues { get; set; }
    public DbSet<Mark> Marks { get; set; }
}
