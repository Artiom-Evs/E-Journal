using E_Journal.SchedulesApi.Models;

namespace E_Journal.SchedulesApi.Services;

public interface ILessonsRepository
{
    IQueryable<Lesson> Lessons { get; }

    bool IsExists(Lesson lesson);
    bool IsExists(int groupId, DateTime date, int number, int subgroup);

    bool Create(Lesson lesson);
    Lesson? Get(int groupId, DateTime date, int number, int subgroup);
    bool Update(Lesson lesson);
    bool Delete(Lesson lesson);
    bool Delete(int groupId, DateTime date, int number, int subgroup);
}
