using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using E_Journal.WebUI.Models;

namespace E_Journal.WebUI.Areas.Admin.Pages;

[Authorize(Roles = ApplicationRoles.Admin)]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
