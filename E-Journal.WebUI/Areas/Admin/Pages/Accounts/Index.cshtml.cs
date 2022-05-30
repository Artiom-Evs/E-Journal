using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Journal.WebUI.Models;

namespace E_Journal.WebUI.Areas.Admin.Pages.Accounts;

[Authorize(Roles = ApplicationRoles.Admin)]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    public int UsersCount { get; private set; }
    public int StudentsCount { get; private set; }
    public int TeachersCount { get; private set; }
    public int UnconfirmedUsersCount { get; private set; }

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        UsersCount = _userManager.Users.Count();
        StudentsCount = (await _userManager.GetUsersInRoleAsync(ApplicationRoles.Student)).Count;
        TeachersCount = (await _userManager.GetUsersInRoleAsync(ApplicationRoles.Teacher)).Count;
        UnconfirmedUsersCount = _userManager.Users
            .Where(u => !u.UserConfirmed)
            .Count();
    }
}
