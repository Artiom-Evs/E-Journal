using E_Journal.Parser;
using E_Journal.Parser.Models;
using E_Journal.SchedulesApi.Extensions;
using System.Runtime.CompilerServices;

namespace E_Journal.SchedulesApi.Services;

public class ParserService : IParserService
{
    private readonly ILogger<ParserService> _logger;

    public ParserService(ILogger<ParserService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<Lesson> ParseDaylySchedulesPage(string pageText)
    {
        IEnumerable<PreparsedTable> preparsedTables;

        try
        {
            preparsedTables = PageParser.ParseDaylySchedules(pageText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ">> Exception occurred while schedule page parsing\r\n");
            yield break;
        }

        foreach (var preparsedTable in preparsedTables)
        {
            IEnumerable<IEnumerable<PreparsedCell>> preparsedCells;

            try
            {
                preparsedCells = TableParser.ParseTable(preparsedTable);
            }
            catch (Exception ex)
            {
                string message = ">> Exception occurred while schedule table parsing\r\n" +
                    preparsedTable.ToErrorString();

                _logger.LogError(ex, message);
                continue;
            }

            foreach (var (dayCells, groupName, date) in preparsedCells.Zip(preparsedTable.ColumnsTitles, preparsedTable.ColumnsDates))
            {
                foreach (var cell in dayCells)
                {
                    List<Lesson> lessons = new();
                    try
                    {
                        lessons.AddRange(CellParser.ParseCell(cell));
                    }
                    catch (Exception ex)
                    {
                        string message = ">> Exception occurred while schedule cell parsing\r\n" +
                                cell.ToErrorString();

                        _logger.LogError(ex, message);
                    }

                    foreach (var lesson in lessons)
                    {
                        yield return lesson;
                    }
                }
            }
        }
    }

    private static bool CompareLessons(IEnumerable<Lesson> sequence1, IEnumerable<Lesson> sequence2)
    {
        HashSet<Lesson> lessonsSet = new(sequence1);
        return lessonsSet.SetEquals(sequence2);
    }
}
