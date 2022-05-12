#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using E_Journal.Shared;
using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;



namespace E_Journal.WebUI.Pages;

[Authorize(Roles = ApplicationRoles.Teacher)]
public class EditLessonTopicModel : PageModel
{
    private readonly IJournalRepository _repository;

    public EditLessonTopicModel(IJournalRepository repository)
    {
        _repository = repository;
    }

    [BindProperty]
    public InputModel Input { get; set; }
    public string ReturnUrl { get; set; }

    public class InputModel
    {
        public int LessonId { get; set; }
        [Display(Name = "Тема занятия")]
        public string Topic { get; set; } = string.Empty;
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;
    }

    public IActionResult OnGet(int lessonId, string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        var lesson = _repository.Lessons
            .SingleOrDefault(l => l.Id == lessonId);

        if (lesson == null)
        {
            return NotFound();
        }

        ViewData["title"] = string.IsNullOrEmpty(lesson.Topic) ? "Добавление темы" : "Изменение темы";

        Input = new InputModel()
        {
            LessonId = lesson.Id, 
            Topic = lesson.Topic, 
            Description = lesson.Description
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var lesson = _repository.Lessons
            .SingleOrDefault(l => l.Id == Input.LessonId);

        if (lesson is null)
        {
            return NotFound();
        }

        lesson.Topic = Input.Topic ?? string.Empty;
        lesson.Description = Input.Description ?? string.Empty;

        await _repository.UpdateAsync<Lesson>(lesson);

        return Redirect(returnUrl);
    }
}
