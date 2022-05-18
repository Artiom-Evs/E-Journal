using Microsoft.EntityFrameworkCore;
using E_Journal.Shared;

namespace E_Journal.Infrastructure
{
    public class JournalDbContext : DbContext
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<ScoreValue> ScoreValues { get; set; }


        public JournalDbContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
