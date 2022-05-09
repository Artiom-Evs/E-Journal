#nullable disable

namespace E_Journal.WebUI.Models.ViewModels;

public record TeacherLessonViewModel
{
    public int LessonId { get; init; }
    public string GroupName { get; init; }
    public string DisciplineName { get; init; }
    public DateTime Date { get; init; }
    public int Number { get; init; }
    public string Room { get; init; }
    public char Subgroup { get; init; }
    public bool HasTopic { get; init; }
}
