using Microsoft.EntityFrameworkCore;
using E_Journal.Shared;

namespace E_Journal.Infrastructure
{
    public class JournalDbContext : DbContext
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<StudentStatus> StudentStatuses { get; set; }
        public DbSet<Timetable> Timetables { get; set; }

        public JournalDbContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
