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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Score>().HasKey(m => new { m.StudentId, m.Date, m.Number });
    }

    public DbSet<Score> Scores { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Models.Type> Types { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<ScoreValue> ScoreValues { get; set; }
}
