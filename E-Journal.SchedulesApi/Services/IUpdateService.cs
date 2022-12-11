using E_Journal.Parser.Models;

namespace E_Journal.SchedulesApi.Services;

public interface IUpdateService
{
    Task<bool> UpdateAsync(IEnumerable<Lesson> parsedLessons);
}
