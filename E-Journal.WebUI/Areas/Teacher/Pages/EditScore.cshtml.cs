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
public class EditScoreModel : PageModel
{
    public class InputModel
    {
        [Required(ErrorMessage = "{0} является обязательным полем")]
        [Display(Name = "Отметка")]
        public string ScoreValue { get; set; } = String.Empty;
    }

    private JournalDbContext _context;


    [BindProperty]
    public InputModel Input { get; set; }
    public int LessonId { get; set; }
    public int StudentId { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
    public string StudentInitials { get; set; }
    public string[] ScoreValues { get; set; } = Array.Empty<string>();

    public EditScoreModel(JournalDbContext context)
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

        var score = _context.Scores
            .SingleOrDefault(s => s.LessonId == lessonId && s.StudentId == studentId);

        if (score == null)
        {
            return NotFound();
        }

        Load(studentId);

        string scoreValue = _context.ScoreValues
            .Single(v => v.Id == score.ValueId)
            .Title;

        Input = new InputModel()
        {
            ScoreValue = scoreValue
        };
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int lessonId = 0, int studentId = 0, string returnUrl = "/")
    {
        if (lessonId <= 0 || studentId <= 0)
        {
            return NotFound();
        }

        LessonId = lessonId;
        StudentId = studentId;
        ReturnUrl = returnUrl;

        var score = _context.Scores
            .SingleOrDefault(s => s.LessonId == lessonId && s.StudentId == studentId);

        if (score == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            Load(studentId);   
            return Page();
        }
        
        var scoreValue = _context.ScoreValues
            .SingleOrDefault(v => v.Title == Input.ScoreValue);
        
        if (scoreValue == null)
        {
            return NotFound();
        }

        score.Value = scoreValue;

        _context.Scores.Update(score);
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
