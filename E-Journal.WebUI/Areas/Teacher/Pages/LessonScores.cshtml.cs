using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

using E_Journal.Shared;
using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Areas.Teacher.Pages;

[Authorize(Roles = ApplicationRoles.Teacher)]
public class LessonScoresModel : PageModel
{
    private JournalDbContext _context;

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
    public IActionResult OnGet(int? lessonId = null)
    {
        var lesson = _context.Lessons
            .SingleOrDefault(l => l.Id == lessonId);

        if (lesson == null)
        {
            return NotFound();
        }

        Date = lesson.Date;
        Topic = lesson.Topic;
        Description = lesson.Description;

        DisciplineName = _context.Disciplines
            .Single(d => d.Id == lesson.DisciplineId)
            .Name;
        TeacherName = _context.Teachers
            .Single(t => t.Id == lesson.TeacherId)
            .Name;
        GroupName = _context.Groups
            .Single(g => g.Id == lesson.GroupId)
            .Name;
        Scores = _context.Scores
            .Where(s => s.LessonId == lessonId)
            .Select(s => new ScoreViewModel()
            {
                ScoreId = s.Id, 
                StudentInitials = s.Student.GetInitials(),
                Value = s.Value
            })
            .ToArray();
        
        return Page();
    }
}
