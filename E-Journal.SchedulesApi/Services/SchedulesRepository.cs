using E_Journal.Parser.Models;

namespace E_Journal.SchedulesApi.Services;

public class SchedulesRepository : ISchedulesRepository
{
    private readonly ApplicationDbContext _context;

    public SchedulesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Lesson> Lessons => _context.Lessons;

    public Lesson Add(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        _context.SaveChanges();
        return lesson;
    }

    public Lesson? Get(int id)
    {
        return _context.Lessons.FirstOrDefault(l => l.Id == id);
    }

    public Lesson? Remove(int id)
    {
        Lesson? lesson = _context.Lessons.FirstOrDefault(l => l.Id == id);

        if (lesson == null)
        {
            return null;
        }

        _context.Lessons.Remove(lesson);
        _context.SaveChanges();
        return lesson;
    }

    public void AddRange(IEnumerable<Lesson> lessons)
    {
        _context.Lessons.AddRange(lessons);
        _context.SaveChanges();
    }

    public void RemoveRange(IEnumerable<Lesson> lessons)
    {
        _context.Lessons.RemoveRange(lessons);
        _context.SaveChanges();
    }
}
