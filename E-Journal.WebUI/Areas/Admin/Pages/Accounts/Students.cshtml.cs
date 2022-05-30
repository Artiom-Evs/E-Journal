using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Areas.Admin.Pages.Accounts;

[Authorize(Roles = ApplicationRoles.Admin)]
public class StudentsModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    public List<ApplicationUserViewModel> Students { get; }

    public StudentsModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        Students = new List<ApplicationUserViewModel>();
    }

    public async Task OnGetAsync()
    {
        await Initialize();
    }

    private async Task Initialize()
    {
        var users = await _userManager.GetUsersInRoleAsync(ApplicationRoles.Student);

        foreach (var user in users)
        {
            Students.Add(new ApplicationUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                IsConfirmed = user.UserConfirmed,
            });
        }
    }
}
