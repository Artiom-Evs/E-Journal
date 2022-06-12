#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using E_Journal.Shared;
using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;

namespace E_Journal.WebUI.Areas.Admin.Pages;

[Authorize(Roles = ApplicationRoles.Admin)]
public class CreateStudentModel : PageModel
{
    public class InputModel
    {
        [Required(ErrorMessage = "{0} является обязательным полем.")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "{0} является обязательным полем.")]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }
        [Required(ErrorMessage = "{0} является обязательным полем.")]
        [Display(Name = "Отчество")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "{0} является обязательным полем.")]
        [Display(Name = "Учебная группа")]
        public string Group { get; set; }
    }

    private JournalDbContext _context;
    [BindProperty]
    public InputModel Input { get; set; }
    public string[] Groups { get; set; }
    public string ReturnUrl { get; set; }

    public CreateStudentModel(JournalDbContext context)
    {
        _context = context;
    }

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? "/";
        Input = new InputModel();
        Load();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? "/";

        Load();

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
            FirstName = Input.FirstName,
            SecondName = Input.SecondName,
            LastName = Input.LastName,
            Group = group
        };

        // TODO: в дальнейшем можно будет попробовать
        // сделать внесение изменений в БД отдельной функцией
        // Администратор сможет добавлять/удалять/редактировать учащихся
        // и только в конце одним действием вносить изменения
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return Redirect(ReturnUrl);
    }

    private void Load()
    {
        Groups = _context.Groups
            .Select(g => g.Name)
            .ToArray();
    }
}
