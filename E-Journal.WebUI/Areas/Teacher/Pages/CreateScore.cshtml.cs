using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using E_Journal.Shared;
using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Areas.Teacher.Pages;

[Authorize(Roles = ApplicationRoles.Teacher)]
public class CreateScoreModel : PageModel
{
    public class InputModel
    {
        [Required(ErrorMessage = "{0} является обязательным полем")]
        [Display(Name = "ФИО учащегося")]
        public int? StudentId { get; set; }
        [Required(ErrorMessage = "{0} является обязательным полем")]
        [Display(Name = "Отметка")]
        public string ScoreValue { get; set; } = String.Empty;
    }

    private JournalDbContext _context;

    public string ReturnUrl { get; set; } = string.Empty;
    public int LessonId { get; set; }
    public string[] ScoreValues { get; set; } = Array.Empty<string>();
    public StudentViewModel[] Students { get; set; } = Array.Empty<StudentViewModel>();
    [BindProperty]
    public InputModel Input { get; set; }

    public CreateScoreModel(JournalDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet(int lessonId = 0, string returnUrl = "/")
    {
        ReturnUrl = returnUrl;

        if (lessonId <= 0)
        {
            return NotFound();
        }

        Load(lessonId);
        LessonId = lessonId;
        Input = new InputModel();
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int lessonId = 0, string returnUrl =  "/")
    {
        ReturnUrl = returnUrl;

        if (lessonId <= 0)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            Load(lessonId);
            LessonId = lessonId;
            return Page();
        }

        var student = _context.Students
            .SingleOrDefault(s => s.Id == Input.StudentId);
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

    private void Load(int lessonId)
    {
        int groupId = _context.Lessons
            .Single(l => l.Id == lessonId)
            .GroupId;

        Students = _context.Students
            .Where(s => s.GroupId == groupId)
            .Select(s => new StudentViewModel()
            {
                Id = s.Id,
                FirstName = s.FirstName,
                SecondName = s.SecondName,
                LastName = s.LastName
            })
            .ToArray();

        ScoreValues = _context.ScoreValues
            .Select(v => v.Title)
            .ToArray();
    }
}
