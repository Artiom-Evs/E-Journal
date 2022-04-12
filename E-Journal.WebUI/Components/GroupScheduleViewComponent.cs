using Microsoft.AspNetCore.Mvc;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;
using E_Journal.Shared;
using E_Journal.Infrastructure;

namespace E_Journal.WebUI.Components
{
    public class GroupScheduleViewComponent : ViewComponent
    {
        private const int STUDY_DAYS_COUNT = 6;
        private readonly IJournalRepository _repository;

        public GroupScheduleViewComponent(IJournalRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke(GroupScheduleDataModel dataModel)
        {
            if (dataModel.ActualScheduleIds.Length > STUDY_DAYS_COUNT)
            {
                throw new ArgumentOutOfRangeException($"Count of actual schedule ids cannot be more then {STUDY_DAYS_COUNT}", nameof(dataModel));
            }

            var actualLessons = GetActualLessonViewModels(dataModel.ActualScheduleIds);

            var maxLessonsCount = GetMaxLessonsCount(actualLessons);

            var someWeekDate = GetSomeWeekDate(actualLessons);
            
            var viewModel = new GroupScheduleViewModel
            {
                GroupName = _repository.Groups.First(g => g.Id == dataModel.GroupId).Name,
                Dates = GetWeekDates(someWeekDate),
                Lessons = actualLessons,
                MaxLessonsCount = maxLessonsCount
            };

            return View(viewModel);
        }

        private LessonViewModel[][] GetActualLessonViewModels(int[] actualScheduleIds)
        {
            List<LessonViewModel[]> entities = new();

            foreach (var id in actualScheduleIds)
            {
                var dayLessons = _repository.Lessons
                    .Where(l => l.ScheduleId == id)
                    .Select(l => new LessonViewModel
                    {
                        DisciplineName = l.Discipline.Name,
                        TeacherName = l.Teacher.Name,
                        Room = l.Room,
                        Date = l.Schedule.Date,
                        Number = l.Number,
                        Subgroup = l.Subgroup
                    })
                    .ToArray();

                entities.Add(dayLessons);
            }

            return entities.ToArray();
        }
        private int GetMaxLessonsCount(LessonViewModel[][] lessons)
        {
            return lessons.DefaultIfEmpty()
                ?.Max(e => e?.DefaultIfEmpty().Max(l => l?.Number ?? 0) ?? 0) ?? 0;
        }
        private DateTime GetSomeWeekDate(LessonViewModel[][] lessons)
        {
            return lessons.FirstOrDefault(l => l.Any())?[0].Date ?? DateTime.Now.Date;
        }
        private DateTime[] GetWeekDates(DateTime someWeekDate)
        {
            DateTime[] dates = new DateTime[STUDY_DAYS_COUNT];

            dates[0] = someWeekDate.AddDays((int)someWeekDate.DayOfWeek * -1 + 1);

            for (int i = 1; i < STUDY_DAYS_COUNT; i++)
            {
                dates[i] = dates[i - 1].AddDays(1);
            }

            return dates;
        }
    }
}
