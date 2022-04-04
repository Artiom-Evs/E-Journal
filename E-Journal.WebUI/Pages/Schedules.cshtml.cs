using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Journal.Infrastructure;
using E_Journal.Shared;

namespace E_Journal.WebUI.Pages
{
    public class SchedulesModel : PageModel
    {
        private readonly ILogger<SchedulesModel> _logger;
        private readonly IJournalRepository _repository;

        public List<Group> Groups { get; }

        public SchedulesModel(ILogger<SchedulesModel> logger, IJournalRepository repository)
        {
            _logger = logger;
            _repository = repository;
            Groups = new List<Group>();
        }

        public void OnGet()
        {
            try
            {
                Groups.AddRange(GetGroupsWithWeekSchedules());
            }
            catch (Exception ex)
            {
                _logger.LogError("Ќе удалось получить расписани€ на неделю.", ex);
            }
        }

        private void SetScheduleLessons(Schedule schedule)
        {
            schedule.Lessons = _repository.Lessons.Where(l => l.ScheduleId == schedule.Id).ToList();
        }

        private DateTime GetStartWeekDay(DateTime date)
        {
            return date.Date.AddDays((int)date.DayOfWeek * -1 + 1);
        }

        private void SetWeekSchedules(Group group)
        {
            DateTime currentWeekStartDate = GetStartWeekDay(DateTime.Now);

            var hasActual = _repository.Schedules.FirstOrDefault(s => s.GroupId == group.Id && s.Date >= currentWeekStartDate)?.Id > 0;

            if (hasActual)
            {
                var actualSchedules = _repository.Schedules
                    .Where(s => s.GroupId == group.Id)
                    .Where(s => s.Date >= currentWeekStartDate)
                    .OrderBy(s => s.Date);
                var lastWeekStartDate = GetStartWeekDay(actualSchedules.Last().Date);
                group.Schedules = actualSchedules.Where(s => s.Date >= lastWeekStartDate).DefaultIfEmpty().ToList();
                group.Schedules.ToList().ForEach(s => SetScheduleLessons(s));
            }
        }

        private Group[] GetGroupsWithWeekSchedules()
        {
            var groups = _repository.Groups.ToList();
            groups.ForEach(g => SetWeekSchedules(g));
            return groups.ToArray();
        }
    }
}
