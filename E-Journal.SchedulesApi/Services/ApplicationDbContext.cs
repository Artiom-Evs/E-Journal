using Microsoft.EntityFrameworkCore;
using E_Journal.Parser.Models;

namespace E_Journal.SchedulesApi.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        Database.Migrate();
    }

    public DbSet<Lesson> Lessons { get; set; }
}
