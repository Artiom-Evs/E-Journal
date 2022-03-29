using E_Journal.Shared;
using E_Journal.Parser;
using E_Journal.Infrastructure;

namespace E_Journal.UpdateService
{
    public class UpdateService : BackgroundService
    {
        private readonly ILogger<UpdateService> _logger;
        private readonly IConfiguration _configuration;
        private readonly JournalDbContext context;
        private readonly HttpClient client;
        private readonly Dictionary<string, int> parseResultHashCodes;
        private readonly TimetableBuilder builder;

        public UpdateService(ILogger<UpdateService> logger, IConfiguration configuration, JournalDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            this.context = context;
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            this.client = new HttpClient();
            builder = new TimetableBuilder(context);
            parseResultHashCodes = new Dictionary<string, int>();
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
        private int[] CheckGroupUpdates(ParseResult[] results)
        {
            List<int> changedGroups = new();

            foreach (var newResult in GetNewParseResults(results))
            {
                Group group = builder.BuildWeekSchedules(newResult);
                bool hasUpdates = UpdateGroup(group);

                if (hasUpdates)
                {
                    context.SaveChanges();
                    changedGroups.Add(group.Id);
                }
            }

            return changedGroups.ToArray();
        }
        private bool UpdateGroup(Group group)
        {
            bool hasChanges = false;

            foreach (var newSchedule in group.Schedules)
            {
                var existSchedule = context.Schedules
                    .FirstOrDefault(s => s.GroupId == newSchedule.GroupId && s.Date == newSchedule.Date);

                if (existSchedule == null)
                {
                    context.Schedules.Add(newSchedule);
                    hasChanges = true;
                }
                else if (!existSchedule.Equals(newSchedule))
                {
                    context.Schedules.Remove(existSchedule);
                    context.Schedules.Add(newSchedule);
                    hasChanges = true;
                }
            }

            return hasChanges;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            context.Database.EnsureCreated();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Update started on: {DateTimeOffset.Now}\n");

                    ParseResult[] results = ScheduleParser.ParseSchedules(await client.GetStringAsync(_configuration["GroupsTimetableAddress"], stoppingToken));

                    int[] changedGroups = CheckGroupUpdates(results);

                    _logger.LogInformation($"Update ended on: {DateTime.Now.ToString()}\r\n" +
                        $"\tUpdated groups: {(changedGroups.Any() ? string.Join(", ", changedGroups.Select(id => context.Groups.First(g => g.Id == id).Name)) : "none")}");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception occured while updating schedules.", ex);
                }

                await Task.Delay(30000, stoppingToken);
            }
        }

        public override void Dispose()
        {
            client.Dispose();
            context.Dispose();
            base.Dispose();
        }
    }
}