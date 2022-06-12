#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using E_Journal.Shared;
using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;

namespace E_Journal.WebUI.Areas.Admin.Pages.Students;

[Authorize(Roles = ApplicationRoles.Admin)]
public class EditStudentModel : PageModel
{
    public class InputModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Id { get; set; } = 0;
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }
        [Required]
        [Display(Name = "Отчество")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Учебная группа")]
        public string Group { get; set; }
    }

    private JournalDbContext _context;
    [BindProperty]
    public InputModel Input { get; set; }
    [BindProperty]
    public string[] Groups { get; set; }
    public string ReturnUrl { get; set; }

    public EditStudentModel(JournalDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet(int? studentId, string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? "/";

        if (studentId == null || studentId <= 0)
        {
            throw new ArgumentException("Parameter cannot be equal to zero.", nameof(studentId));
        }

        var student = _context.Students
            .SingleOrDefault(g => g.Id == studentId);

        if (student == null)
        {
            return NotFound();
        }

        Load(student);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? "/";

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Group group = _context.Groups.Single(g => g.Name == Input.Group);

        if (group == null)
        {
            return NotFound();
        }

        E_Journal.Shared.Student student = new()
        {
            Id = Input.Id,
            FirstName = Input.FirstName,
            SecondName = Input.SecondName,
            LastName = Input.LastName,
            Group = group
        };

        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        return Redirect(ReturnUrl);
    }

    private void Load(E_Journal.Shared.Student student)
    {
        Groups = _context.Groups.Select(g => g.Name).ToArray();

        Input = new InputModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            SecondName = student.SecondName,
            LastName = student.LastName,
            Group = _context.Groups
                .Single(g => g.Id == student.GroupId)
                .Name
        };
    }
}
