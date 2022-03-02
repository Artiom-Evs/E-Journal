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
            this.client = new HttpClient();
            schedulesAddress = configuration["GroupsTimetableAddress"];
            schedulesHashCodes = new Dictionary<string, int>();
            builder = new TimetableBuilder(new JournalRepository(context));
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Start updating at: {DateTimeOffset.Now}\n");

                try
                {
                    string pageText = await client.GetStringAsync(schedulesAddress);
                    ParseResult[] results = TimetableParser.ParseTimetable(pageText);
                    
                    foreach (var schedule in GetOutdatedSchedules(results))
                    {
                        Group group = await builder.BuildGroup(schedule);
                        context.Groups.Update(group);
                        await context.SaveChangesAsync();
                    }

                    _logger.LogInformation($"Update ended at: {DateTimeOffset.Now}\n");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception occured while updating timetable.", ex);
                }
                
                await Task.Delay(10000, stoppingToken);
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