using E_Journal.Shared;
using E_Journal.Parser;
using E_Journal.Infrastructure;

namespace E_Journal.UpdateService
{
    public class UpdateService : BackgroundService
    {
        private readonly ILogger<UpdateService> _logger;
        private readonly JournalDbContext context;
        private readonly HttpClient client;
        private readonly string schedulesAddress;
        private readonly Dictionary<string, int> schedulesHashCodes;
        private readonly TimetableBuilder builder;

        public UpdateService(ILogger<UpdateService> logger, JournalDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            this.context = context;
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            this.client = new HttpClient();
            schedulesAddress = configuration["GroupsTimetableAddress"];
            schedulesHashCodes = new Dictionary<string, int>();
            builder = new TimetableBuilder(context);
        }

        private int CheckHashCode(string groupName)
        {
            if (schedulesHashCodes.ContainsKey(groupName))
            {
                return schedulesHashCodes[groupName];
            }
            else
            {
                return -1;
            }
        }
        private IEnumerable<ParseResult> GetOutdatedSchedules(ParseResult[] results)
        {
            foreach (var result in results)
            {
                if (CheckHashCode(result.Name) != result.HashCode)
                {
                    yield return result;
                    schedulesHashCodes[result.Name] = result.HashCode;
                }
            }
        }

        private bool UpdateSchedules(Group group)
        {
            bool hasChanges = false;

            foreach (var newSchedule in group.Schedules)
            {
                bool isExists = context.Schedules
                    .Where(s => s.GroupId == group.Id && s.Date == newSchedule.Date)
                    .Any();

                if (!isExists)
                {
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
                _logger.LogInformation($"Start updating at: {DateTimeOffset.Now}\n");

                try
                {
                    string pageText = await client.GetStringAsync(schedulesAddress, stoppingToken);
                    ParseResult[] results = ScheduleParser.ParseSchedules(pageText);
                    List<string> changedGroups = new();

                    foreach (var textSchedule in GetOutdatedSchedules(results))
                    {
                        Group group = builder.BuildWeekSchedules(textSchedule);

                        if (UpdateSchedules(group))
                        {
                            context.SaveChanges();
                            changedGroups.Add(group.Name);
                        }
                    }

                    _logger.LogInformation($"Update ended at: {DateTimeOffset.Now}\r\n" +
                        $"\tUpdated groups: {(changedGroups.Any() ? string.Join(", ", changedGroups) : "none")}");
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