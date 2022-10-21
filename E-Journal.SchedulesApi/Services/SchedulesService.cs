using E_Journal.Parser;
using E_Journal.Parser.Models;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Writers;
using System.Collections.Immutable;

namespace E_Journal.SchedulesApi.Services;

public class SchedulesService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public SchedulesService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int checkFrequency;

        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            var configuraion = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (!int.TryParse(configuraion["UPDATE_CHECK_FREQUENCY_MINUTES"], out checkFrequency))
            {
                throw new InvalidOperationException("ENVIRONMENT do not contasin 'UPDATE_CHECK_FREQUENCY_MINUTES' variable.");
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateSchedulesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(checkFrequency), stoppingToken);
        }
    }


    public async Task UpdateSchedulesAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISchedulesRepository repository = scope.ServiceProvider.GetRequiredService<ISchedulesRepository>();

        string uri = @"http://mgke.minsk.edu.by/ru/main.aspx?guid=3841";
        string pageText = await (new HttpClient()).GetStringAsync(uri);
        
        var preparsedTables = PageParser.ParseDaylySchedules(pageText);

        foreach (var preparsedTable in preparsedTables)
        {
            var preparsedCells = TableParser.ParseTable(preparsedTable);

            foreach (var (dayCells, groupName, date) in preparsedCells.Zip(preparsedTable.ColumnsTitles, preparsedTable.ColumnsDates))
            {
                var lessons = CellParser.ParseCells(dayCells);

                var storedLessons = repository.Lessons
                    .Where(l => l.Date == date && l.GroupName == groupName)
                    .ToList();

                if (CompareLessons(lessons, storedLessons))
                {
                    continue;
                }

                if (storedLessons.Any())
                {
                    repository.RemoveRange(storedLessons);
                }

                if (lessons.Any())
                {
                    repository.AddRange(lessons);
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
