using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Journal.Infrastructure;
using E_Journal.Shared;
using E_Journal.WebUI.Models;

namespace E_Journal.WebUI.Pages
{
    public class SchedulesModel : PageModel
    {
        private const int STUDY_DAYS_COUNT = 6;
        private readonly ILogger<SchedulesModel> _logger;
        public readonly IJournalRepository _repository;

        public List<GroupScheduleDataModel> GroupDataModels { get; }

        public SchedulesModel(ILogger<SchedulesModel> logger, IJournalRepository repository)
        {
            _logger = logger;
            _repository = repository;
            GroupDataModels = new List<GroupScheduleDataModel>();
        }

        public void OnGet()
        {
            try
            {
                foreach (var group in _repository.Groups)
                {
                    GroupDataModels.Add(new GroupScheduleDataModel()
                    {
                        GroupId = group.Id,
                        ActualScheduleIds = GetActualScheduleIds(group)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ќе удалось получить расписани€ на неделю.", ex);
            }
        }

        private int[] GetActualScheduleIds(Group group)
        {
            var lastDate = _repository.Schedules
                .Where(s => s.GroupId == group.Id)
                .DefaultIfEmpty()
                ?.OrderBy(s => s.Date)
                ?.Last()
                ?.Date ?? default;

            var lastWeekStartDate = GetStartWeekDay(lastDate);

            if (lastWeekStartDate < GetStartWeekDay(DateTime.Now.Date))
            {
                return Array.Empty<int>();
            }

            var actualScheduleIds = _repository.Schedules
                .Where(s => s.GroupId == group.Id)
                .Where(s => s.Date >= lastWeekStartDate)
                .Select(s => s.Id)
                .ToArray();

            return actualScheduleIds;
        }

        private DateTime GetStartWeekDay(DateTime date)
        {
            return date.Date.AddDays((int)date.DayOfWeek * -1 + 1);
        }
    }
}
