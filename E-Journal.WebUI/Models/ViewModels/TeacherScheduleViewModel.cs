#nullable disable
using E_Journal.Shared;

namespace E_Journal.WebUI.Models.ViewModels;

public record TeacherScheduleViewModel
{
    public string Title { get; init; }
    public DateTime[] Dates { get; init; }
    public TeacherLessonViewModel[] Lessons { get; init; }
    public int MaxLessonsPerDay { get; init; }
}
