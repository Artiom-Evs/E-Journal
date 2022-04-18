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
                    _logger.LogError("Exception occured while updating schedules.", ex);
                }

                await Task.Delay(_configuration.GetValue<int>("UPDATE_FREQUENCY_MS"), stoppingToken);
            }
        }

        private async Task<int[]> StartUpdating(CancellationToken stoppingToken)
        {
            List<int> changedGroupIds = new();
            string pageText = await client.GetStringAsync(_configuration["GroupsTimetableAddress"], stoppingToken);
            ParseResult[] results = ScheduleParser.ParseSchedules(pageText);
            var newParseResults = GetNewParseResults(results);

            if (!newParseResults.Any())
            {
                return changedGroupIds.ToArray();
            }

            Schedule[][] updateCandidates = newParseResults
                .Select(r => builder.BuildWeekSchedules(r))
                .Select(g => g.Schedules.ToArray())
                .ToArray();

            foreach (var groupUpdateCandidates in updateCandidates)
            {
                var newSchedules = GetNewSchedules(groupUpdateCandidates);

                if (!newSchedules.Any())
                {
                    continue;
                }

                AddNewSchedules(newSchedules.Where(s => s.Item1 == null).Select(s => s.Item2));
                ReplaceOldSchedules(newSchedules.Where(s => s.Item1 != null));

                _context.SaveChanges();

                changedGroupIds.Add(newSchedules.First().Item2.Group.Id);
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
        private (Schedule?, Schedule)[] GetNewSchedules(Schedule[] newSchedules)
        {
            List<(Schedule?, Schedule)> itemsToReplace = new();

            foreach (var newSchedule in newSchedules)
            {
                var existSchedule = _context.Schedules
                    .Include(s => s.Lessons)
                    .FirstOrDefault(s => s.GroupId == newSchedule.Group.Id && s.Date == newSchedule.Date);

                if (existSchedule == null)
                {
                    itemsToReplace.Add(new(null, newSchedule));
                }
                else if (!IsSchedulesEqual(existSchedule, newSchedule))
                {
                    itemsToReplace.Add(new(existSchedule, newSchedule));
                }
            }

            return itemsToReplace.ToArray();
        }
        private bool IsSchedulesEqual(Schedule x, Schedule y)
        {
            if (x.Group.Id != y.Group.Id || x.Date != y.Date || x.Lessons.Count != y.Lessons.Count)
            {
                return false;
            }

            foreach (var (l1, l2) in x.Lessons.Zip(y.Lessons))
            {
                if (l1.Discipline.Id != l2.Discipline.Id ||
                    l1.Teacher.Id != l2.Teacher.Id ||
                    l1.Room != l2.Room ||
                    l1.Subgroup != l2.Subgroup)
                {
                    return false;
                }
            }

            return true;
        }
        private void AddNewSchedules(IEnumerable<Schedule> schedules)
        {
            _context.AddRange(schedules);
        }
        private void ReplaceOldSchedules(IEnumerable<(Schedule?, Schedule)> schedules)
        {
            foreach (var (OldSchedule, NewSchedule) in schedules)
            {
                if (OldSchedule != null)
                {
                    _context.Schedules.Remove(OldSchedule);
                    _context.Schedules.Add(NewSchedule);
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