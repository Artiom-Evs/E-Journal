#nullable disable

namespace E_Journal.WebUI.Models.ViewModels
{
    public record LessonViewModel
    {
        public string TeacherName { get; init; }
        public string DisciplineName { get; init; }
        public string Room { get; init; }
        public DateTime Date { get; init; }
        public int Number { get; init; }
        public char Subgroup { get; init; }
    }
}
