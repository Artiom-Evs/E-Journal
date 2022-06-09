using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Journal.Infrastructure;
using E_Journal.Shared;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Pages
{
    public class SchedulesModel : PageModel
    {
        private readonly ILogger<SchedulesModel> _logger;
        public readonly IJournalRepository _repository;

        public List<GroupScheduleViewModel> GroupSchedules { get; }

        public SchedulesModel(ILogger<SchedulesModel> logger, IJournalRepository repository)
        {
            _logger = logger;
            _repository = repository;
            GroupSchedules= new List<GroupScheduleViewModel>();
        }

        public void OnGet()
        {
            try
            {
                foreach (var group in _repository.Groups)
                {
                    DateTime firstDateOfLastWeek = GetFirstWeekDate(GetLastLessonDate(group.Id));
                    DateTime[] lastWeekDates = GetWeekDates(firstDateOfLastWeek);

                    StudentLessonViewModel[] lessons = _repository.Lessons
                        .Where(l => l.Date >= firstDateOfLastWeek && l.GroupId == group.Id)
                        .Select(l => new StudentLessonViewModel()
                        {
                            DisciplineName = l.Discipline.Name, 
                            TeacherName = l.Teacher.Name,
                            Date = l.Date,
                            Number = l.Number,
                            Room = l.Room, 
                            Subgroup = l.Subgroup
                        })
                        .ToArray();

                    int maxLessonsPerDay = GetMaxLessonsPerDay(lessons);

                    GroupSchedules.Add(new GroupScheduleViewModel()
                    {
                        Title = $"Группа - {group.Name}", 
                        Dates = lastWeekDates, 
                        Lessons = lessons, 
                        MaxLessonsPerDay = maxLessonsPerDay
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Не удалось получить расписания на неделю.", ex);
            }
        }

        public DateTime GetLastLessonDate(int groupId) =>
            _repository.Lessons
                .Where(l => l.GroupId == groupId)
                .OrderBy(l => l.Date)
                .LastOrDefault()
                ?.Date ?? DateTime.Now.Date;

        private DateTime GetFirstWeekDate(DateTime date)
        {
            return date.Date.AddDays((int)date.DayOfWeek * -1 + 1);
        }

        private DateTime[] GetWeekDates(DateTime someDateOfRequiredWeek)
        {
            DateTime firstDateOfWeek = GetFirstWeekDate(someDateOfRequiredWeek);

            return new[]
            {
                firstDateOfWeek,
                firstDateOfWeek.AddDays(1),
                firstDateOfWeek.AddDays(2),
                firstDateOfWeek.AddDays(3),
                firstDateOfWeek.AddDays(4),
                firstDateOfWeek.AddDays(5)
            };
        }

        private int GetMaxLessonsPerDay(StudentLessonViewModel[] lessons) =>
            lessons.DefaultIfEmpty().Max(l => l?.Number ?? 0);
    }
}
