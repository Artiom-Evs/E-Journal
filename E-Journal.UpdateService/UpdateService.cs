using Microsoft.EntityFrameworkCore;
using E_Journal.Shared;
using E_Journal.Parser;
using E_Journal.Infrastructure;

namespace E_Journal.UpdateService
{
    public class UpdateService : BackgroundService
    {
        private readonly ILogger<UpdateService> _logger;
        private readonly IConfiguration _configuration;
        private readonly JournalDbContext _context;
        private readonly HttpClient client;
        private readonly Dictionary<string, int> parseResultHashCodes;
        private readonly TimetableBuilder builder;

        public UpdateService(ILogger<UpdateService> logger, IConfiguration configuration, JournalDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            this._context = context;
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            this.client = new HttpClient();
            builder = new TimetableBuilder(context);
            parseResultHashCodes = new Dictionary<string, int>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _context.Database.EnsureCreated();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Update started on: {DateTimeOffset.Now}");

                    int[] changedGroupIds = await StartUpdating(stoppingToken);

                    _logger.LogInformation($"Update ended on: {DateTime.Now.ToString()}. " +
                        $"Updated groups: {(changedGroupIds.Any() ? string.Join(", ", changedGroupIds.Select(id => _context.Groups.First(g => g.Id == id).Name)) : "none")}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception occured while updating schedules.\r\n" +
                        $"Message: {ex.ToString()}\r\n" +
                        $"Stack Trace: {ex.StackTrace}", ex);
                }

                await Task.Delay(_configuration.GetValue<int>("UPDATE_FREQUENCY_MS"), stoppingToken);
            }
        }

        private async Task<int[]> StartUpdating(CancellationToken stoppingToken)
        {
            List<int> changedGroupIds = new();
            
            // ��������� ���-�������� ����������
            var responce = await client.GetAsync(_configuration["GroupsTimetableAddress"], HttpCompletionOption.ResponseContentRead, stoppingToken);
            responce.EnsureSuccessStatusCode();
            string pageText = await responce.Content.ReadAsStringAsync(stoppingToken);

            // ��������� ����������� �������� ���-�������� ���������. 
            ParseResult[] results = ScheduleParser.ParseSchedules(pageText);
            ParseResult[] newParseResults = GetNewParseResults(results).ToArray();

            // ���� ���� ����������� �������� ��������� � ����������� ������ - ������ �� ������. 
            if (!newParseResults.Any())
            {
                return Array.Empty<int>(); 
            }

            // ������� ����� ����������. 
            foreach (var newResult in newParseResults)
            {
                // ���� �� ������� ���� - ���������� �����������. 
                if (newResult.Days is null)
                {
                    continue;
                }

                // ���������� � ����� �������
                var parsedLessons = builder.BuildWeekLessons(newResult);

                // ���� �� ����� �� ������� ������� - ������ ������ �� ����. 
                if (!parsedLessons.Any())
                {
                    continue;
                }

                var groupId = _context.Groups
                    .Single(g => g.Name == newResult.Name)
                    .Id;

                // ��������� ������� �� �� �� ����
                var oldLessons = _context.Lessons
                    .Where(l => l.Date >= newResult.Days.First() && l.Date <= newResult.Days.Last())
                    .Where(l => l.GroupId == groupId)
                    .ToArray();

                // ��� ����� �������. ��� ������ �����, ��� � ����������
                var newLessons = parsedLessons
                    .Except(oldLessons)
                    .ToArray();

                // ���� ����� ������� ��� - ������ ������ �� ����. 
                if (!newLessons.Any())
                {
                    continue;
                }

                // �������� ���������� �������, ��� ����� � ���������� ������ ����������� ���������. 
                // ������� �� ������� ������, ����, ������ ���� � �������� ����������. 
                // ��� ��� ���� ��� ��������.
                var outdatedLessons = oldLessons
                    .Intersect(newLessons, new LessonEqualityComparer())
                    .ToArray();

                // ��������� ���������. 
                _context.Lessons.RemoveRange(outdatedLessons);
                await _context.Lessons.AddRangeAsync(newLessons, stoppingToken);
                await _context.SaveChangesAsync(stoppingToken);

                changedGroupIds.Add(groupId);
            }

            return changedGroupIds.ToArray();
        }

        private IEnumerable<ParseResult> GetNewParseResults(ParseResult[] results)
        {
            foreach (var result in results)
            {
                if ((parseResultHashCodes.ContainsKey(result.Name) ? parseResultHashCodes[result.Name] : -1) != result.HashCode)
                {
                    yield return result;
                    parseResultHashCodes[result.Name] = result.HashCode;
                }
            }
        }
        
        public override void Dispose()
        {
            client.Dispose();
            _context.Dispose();
            base.Dispose();
        }
    }
}