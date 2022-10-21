using E_Journal.Parser.Models;

namespace E_Journal.SchedulesApi.Services;

public interface ISchedulesRepository
{
    IQueryable<Lesson> Lessons { get; }

    Lesson Add(Lesson lesson);
    Lesson? Get(int id);
    Lesson? Remove(int id);
    void AddRange(IEnumerable<Lesson> lessons);
    void RemoveRange(IEnumerable<Lesson> lessons);
}
