using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using E_Journal.Shared;
using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Areas.Teacher.Pages;

// TODO: подумать, как обработать ситуацию, в которой оценка у студента уже есть,
// но при этом возникает попытка выставить ещё одну оценку 
[Authorize(Roles = ApplicationRoles.Teacher)]
public class CreateScoreModel : PageModel
{
    public class InputModel
    {
        [Required(ErrorMessage = "{0} является обязательным полем")]
        [Display(Name = "Отметка")]
        public string ScoreValue { get; set; } = String.Empty;
    }

    private JournalDbContext _context;

    public string ReturnUrl { get; set; } = string.Empty;
    public int LessonId { get; set; }
    public int StudentId { get; set; }
    public string StudentInitials { get; set; }
    public string[] ScoreValues { get; set; } = Array.Empty<string>();
    [BindProperty]
    public InputModel Input { get; set; }

    public CreateScoreModel(JournalDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet(int lessonId = 0, int studentId = 0, string returnUrl = "/")
    {
        if (lessonId <= 0 || studentId <= 0)
        {
            return NotFound();
        }

        LessonId = lessonId;
        StudentId = studentId;
        ReturnUrl = returnUrl;

        Load(studentId);
        Input = new InputModel();
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int lessonId = 0, int studentId = 0, string returnUrl =  "/")
    {
        if (lessonId <= 0 || studentId <= 0)
        {
            return NotFound();
        }

        LessonId = lessonId;
        StudentId = studentId;
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            Load(studentId);
            return Page();
        }

        var student = _context.Students
            .SingleOrDefault(s => s.Id == StudentId);
        var scoreValue = _context.ScoreValues
            .SingleOrDefault(v => v.Title == Input.ScoreValue);
        var lesson = _context.Lessons
            .SingleOrDefault(l => l.Id == lessonId);

        if (student == null || scoreValue == null || lesson == null)
        {
            return NotFound();
        }

        Score score = new()
        {
            Student = student,
            Value = scoreValue,
            Lesson = lesson
        };

        await _context.Scores.AddAsync(score);
        await _context.SaveChangesAsync();

        return Redirect(returnUrl);
    }

    private void Load(int studentId)
    {
        StudentInitials = _context.Students
            .Single(s => s.Id == studentId)
            .GetInitials();

        ScoreValues = _context.ScoreValues
            .Select(v => v.Title)
            .ToArray();
    }
}
