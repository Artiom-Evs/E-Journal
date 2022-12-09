using E_Journal.Parser.Models;

namespace E_Journal.SchedulesApi.Services;

public interface IParserService
{
    IEnumerable<Lesson> ParseDaylySchedulesPage(string pageText);
}
