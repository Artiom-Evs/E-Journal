#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using E_Journal.Shared;
using E_Journal.Infrastructure;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Areas.Admin.Pages.Students;

[Authorize(Roles = ApplicationRoles.Admin)]
public class IndexModel : PageModel
{
    public class StudentsViewModel
    {
        public PaginingViewModel PaginingInfo { get; set; }
        public string[] Groups { get; set; }
        public StudentViewModel[] Students { get; set; }
    }
    
    private JournalDbContext _context;
    public StudentsViewModel ViewModel { get; }
    
    public IndexModel(JournalDbContext context)
    {
        _context = context;
        ViewModel = new StudentsViewModel();
        ViewModel.PaginingInfo = new PaginingViewModel();
    }

    public void OnGet(int currentPage = 1)
    {
        if (currentPage <= 0)
        {
            throw new ArgumentException("Parameter cannot be less then or equal to zero. ", nameof(currentPage));
        }

        Load(currentPage);
    }
    
    public async Task<IActionResult> OnGetDeleteStudentAsync(int studentId, int currentPage = 1)
    {
        if (currentPage <= 0)
        {
            throw new ArgumentException("Parameter cannot be less then or equal to zero. ", nameof(currentPage));
        }
        
        E_Journal.Shared.Student student = _context.Students
            .Single(s => s.Id == studentId);

        if (student == null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { currentPage });
    }

    private void Load(int currentPage)
    {
        ViewModel.PaginingInfo.ItemsPerPage = 10;
        ViewModel.PaginingInfo.CurrentPage = currentPage;
        ViewModel.PaginingInfo.ItemsCount = _context.Students.Count();
        ViewModel.Groups = _context.Groups.Select(g => g.Name).ToArray();

        ViewModel.Students = _context.Students
            .Skip(ViewModel.PaginingInfo.ItemsPerPage * (ViewModel.PaginingInfo.CurrentPage - 1))
            .Take(ViewModel.PaginingInfo.ItemsPerPage)
            .Select(s => new StudentViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                SecondName = s.SecondName,
                LastName = s.LastName,
                Group = s.Group.Name
            })
            .ToArray();
    }
}
