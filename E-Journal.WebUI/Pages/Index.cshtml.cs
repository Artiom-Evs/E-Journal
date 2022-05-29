using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Journal.WebUI.Models;

namespace E_Journal.WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (User.IsInRole(ApplicationRoles.Student))
            {
                return RedirectToRoute(new { area = ApplicationRoles.Student });
            }
            else if (User.IsInRole(ApplicationRoles.Teacher))
            {
                return RedirectToRoute(new { area = ApplicationRoles.Teacher });
            }
            else if (User.IsInRole(ApplicationRoles.Admin))
            {
                return RedirectToRoute(new { area = ApplicationRoles.Admin });
            }

            return Page();
        }
    }
}