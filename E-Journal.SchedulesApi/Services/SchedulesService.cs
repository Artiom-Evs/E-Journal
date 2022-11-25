using E_Journal.Parser;
using E_Journal.Parser.Models;
using E_Journal.SchedulesApi.Extensions;

namespace E_Journal.SchedulesApi.Services;

public class SchedulesService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SchedulesService> _logger;
    
    public SchedulesService(IServiceScopeFactory scopeFactory, ILogger<SchedulesService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int checkFrequency;
        string uri1 = string.Empty;
        string uri2 = string.Empty;

        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            uri1 = configuration["BUILDING_1_GROUP_DAYLY_SCHEDULE_URL"];
            uri2 = configuration["BUILDING_2_GROUP_DAYLY_SCHEDULE_URL"];

            if (!int.TryParse(configuration["UPDATE_CHECK_FREQUENCY_MINUTES"], out checkFrequency))
            {
                throw new InvalidOperationException("ENVIRONMENT do not contasin 'UPDATE_CHECK_FREQUENCY_MINUTES' variable.");
            }

            if (string.IsNullOrEmpty(uri1))
            {
                throw new InvalidOperationException("ENVIRONMENT do not contasin 'BUILDING_1_GROUP_DAYLY_SCHEDULE_URL' variable.");
            }

            if (string.IsNullOrEmpty(uri2))
            {
                throw new InvalidOperationException("ENVIRONMENT do not contasin 'BUILDING_2_GROUP_DAYLY_SCHEDULE_URL' variable.");
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(">> Update of the dayly schedules of the building 1 has started");
            await UpdateDaylySchedulesAsync(uri1, stoppingToken);
            _logger.LogInformation(">> Update of the dayly schedules of the building 1 has ended");

            _logger.LogInformation(">> Update of the dayly schedules of the building 2 has started");
            await UpdateDaylySchedulesAsync(uri2, stoppingToken);
            _logger.LogInformation(">> Update of the dayly schedules of the building 2 has ended");

            await Task.Delay(TimeSpan.FromMinutes(checkFrequency), stoppingToken);
        }
    }

    public async Task UpdateDaylySchedulesAsync(string url, CancellationToken stoppingToken)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) && uri != null)
        {
            throw new ArgumentException($"Uncorrect URL format: '{url}'");
        }

        using IServiceScope scope = _scopeFactory.CreateScope();

        ISchedulesRepository repository = scope.ServiceProvider.GetRequiredService<ISchedulesRepository>();
        
        string pageText = await new HttpClient().GetStringAsync(uri, stoppingToken);

        IEnumerable<PreparsedTable> preparsedTables;

        try
        {
            preparsedTables = PageParser.ParseDaylySchedules(pageText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ">> Exception occurred while schedule page parsing\r\n");
            return;
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
                List<Lesson> lessons = new();

                foreach (var cell in dayCells)
                {
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
                }

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
