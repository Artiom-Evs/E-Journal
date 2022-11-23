using E_Journal.Parser;
using E_Journal.Parser.Models;

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

        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            var configuraion = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (!int.TryParse(configuraion["UPDATE_CHECK_FREQUENCY_MINUTES"], out checkFrequency))
            {
                throw new InvalidOperationException("ENVIRONMENT do not contasin 'UPDATE_CHECK_FREQUENCY_MINUTES' variable.");
            }
            
            if (string.IsNullOrEmpty(configuraion["GROUP_DAYLY_SCHEDULE_URL"]))
            {
                throw new InvalidOperationException("ENVIRONMENT do not contasin 'GROUP_DAYLY_SCHEDULE_URL' variable.");
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(">> Update started");
            await UpdateSchedulesAsync(stoppingToken);
            _logger.LogInformation(">> Update ended");

            await Task.Delay(TimeSpan.FromMinutes(checkFrequency), stoppingToken);
        }
    }

    public async Task UpdateSchedulesAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        ISchedulesRepository repository = scope.ServiceProvider.GetRequiredService<ISchedulesRepository>();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        string uri = configuration["GROUP_DAYLY_SCHEDULE_URL"];
        string pageText = await new HttpClient().GetStringAsync(uri, stoppingToken);
        
        var preparsedTables = PageParser.ParseDaylySchedules(pageText);

        foreach (var preparsedTable in preparsedTables)
        {
            var preparsedCells = TableParser.ParseTable(preparsedTable);

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
                        string message = $">> Exception occurred while schedule cell parsing" +
                                $"\r\nNot parsed data:" +
                                $"\r\nLesson cell: {cell.LessonCell.InnerHtml.Trim()}" +
                                $"\r\nRoom cell: {cell.RoomCell.InnerHtml.Trim()}" +
                                $"\r\nLesson number: {cell.LessonNumber}" +
                                $"\r\nLesson date: {cell.LessonDate}" +
                                $"\r\nSchedule header: {cell.ScheduleHeader}" +
                                $"\r\n";

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
