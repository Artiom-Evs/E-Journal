using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Journal.WebUI.Areas.Teacher.Pages;

[Authorize(Roles = ApplicationRoles.Teacher)]
public class LessonScoresModel : PageModel
{
    private JournalDbContext _context;

    public int LessonId { get; set; }
    public string DisciplineName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ScoreViewModel[] Scores { get; set; } = Array.Empty<ScoreViewModel>();

    public LessonScoresModel(JournalDbContext context)
    {
        _context = context;
    }

    // TODO: уточнить, нужно ли контроллировать какой преподаватель выставляет оценки
    public IActionResult OnGet(int lessonId = 0)
    {
        var lesson = _context.Lessons
            .SingleOrDefault(l => l.Id == lessonId);

        if (lesson == null)
        {
            return NotFound();
        }

        var group = _context.Groups
            .Single(g => g.Id == lesson.GroupId);

        LessonId = lesson.Id;
        Date = lesson.Date;
        Topic = lesson.Topic;
        Description = lesson.Description;

        DisciplineName = _context.Disciplines
            .Single(d => d.Id == lesson.DisciplineId)
            .Name;
        TeacherName = _context.Teachers
            .Single(t => t.Id == lesson.TeacherId)
            .Name;
        GroupName = group.Name;

        List<ScoreViewModel> scoreList = new();

        foreach (var student in _context.Students.Where(s => s.GroupId == group.Id))
        {
            var score = _context.Scores
                .SingleOrDefault(s => s.LessonId == lessonId && s.StudentId == student.Id);

            scoreList.Add(new ScoreViewModel()
            {
                StudentId = student.Id,
                StudentInitials = student.GetInitials(),
                Value = score == null ? "-" : _context.ScoreValues.Single(s => s.Id == score.ValueId).Title,
                IsMarked = score != null,
            });
        }

        Scores = scoreList.ToArray();

        return Page();
    }

    public async Task<IActionResult> OnGetDeleteScoreAsync(int lessonId = 0, int studentId = 0)
    {
        var score = _context.Scores
            .SingleOrDefault(s => s.LessonId == lessonId && s.StudentId == studentId);

        if (score == null)
        {
            return NotFound();
        }

        _context.Scores.Remove(score);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { lessonId });
    }
}
