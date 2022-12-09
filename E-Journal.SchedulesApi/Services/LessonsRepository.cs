using E_Journal.SchedulesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.SchedulesApi.Services;

public class LessonsRepository : ILessonsRepository
{
    protected readonly ApplicationDbContext _context;

    public LessonsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Lesson> Lessons => _context.Lessons
        .Include(l => l.Group)
        .Include(l => l.Subject)
        .Include(l => l.Type)
        .Include(l => l.Teacher)
        .Include(l => l.Room);

    public bool IsExists(Lesson lesson)
    {
        return this.IsExists(lesson.GroupId, lesson.Date, lesson.Number, lesson.Subgroup);
    }
    public bool IsExists(int groupId, DateTime date, int number, int subgroup)
    {
        return _context.Lessons.Any(l =>
            l.GroupId == groupId &&
            l.Date == date &&
            l.Number == number &&
            l.Subgroup == subgroup);
    }

    public bool Create(Lesson lesson)
    {
        if (this.IsExists(lesson))
        {
            return false;
        }

        _context.Lessons.Add(lesson);
        _context.SaveChanges();
        return true;
    }

    public Lesson? Get(int groupId, DateTime date, int number, int subgroup)
    {
        return _context.Lessons.FirstOrDefault(l =>
            l.GroupId == groupId &&
            l.Date == date &&
            l.Number == number &&
            l.Subgroup == subgroup);
    }

    public bool Update(Lesson lesson)
    {
        var storedLesson = this.Get(lesson.GroupId, lesson.Date, lesson.Number, lesson.Subgroup);

        if (storedLesson == null)
        {
            return false;
        }

        if (storedLesson.Subject.Name != lesson.Subject.Name)
        {
            storedLesson.Subject = lesson.Subject;
        }

        if (storedLesson.Type.Name != lesson.Type.Name)
        {
            storedLesson.Type = lesson.Type;
        }

        if (storedLesson.Teacher.Name != lesson.Teacher.Name)
        {
            storedLesson.Teacher = lesson.Teacher;
        }

        if (storedLesson.Room.Name != lesson.Room.Name)
        {
            storedLesson.Room = lesson.Room;
        }

        if (storedLesson.Type.Name != lesson.Type.Name)
        {
            storedLesson.Type = lesson.Type;
        }

        _context.Lessons.Update(storedLesson);
        _context.SaveChanges();
        return true;
    }

    public bool Delete(Lesson lesson)
    {
        return this.Delete(lesson.GroupId, lesson.Date, lesson.Number, lesson.Subgroup);
    }
    public bool Delete(int groupId, DateTime date, int number, int subgroup)
    {
        var lesson = this.Get(groupId, date, number, subgroup);

        if (lesson == null)
        {
            return false;
        }

        _context.Lessons.Remove(lesson);
        _context.SaveChanges();
        return true;
    }
}