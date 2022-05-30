using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Areas.Admin.Pages.Accounts;

[Authorize(Roles = ApplicationRoles.Admin)]
public class UnconfirmedUsersModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    public List<ApplicationUserViewModel> UnconfirmedUsers { get; }

    public UnconfirmedUsersModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        UnconfirmedUsers = new List<ApplicationUserViewModel>();
    }

    public async Task OnGetAsync()
    {
        await Initialize();
    }

    public async Task<IActionResult> OnGetConfirmUserAsync(string userId)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            user.UserConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        await Initialize();
        return Page();
    }

    private async Task Initialize()
    {
        var users = _userManager.Users
            .Where(u => !u.UserConfirmed)
            .ToArray();

        foreach (var user in users)
        {
            string role = (await _userManager.GetRolesAsync(user))
                .SingleOrDefault("Некорректное определение роли");

            UnconfirmedUsers.Add(new ApplicationUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                IsConfirmed = user.UserConfirmed,
                Role = role
            });
        }
    }
}
