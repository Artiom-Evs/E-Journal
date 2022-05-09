using Microsoft.AspNetCore.Mvc;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;
using E_Journal.Shared;
using E_Journal.Infrastructure;

namespace E_Journal.WebUI.Components
{
    public class GroupScheduleViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(GroupScheduleViewModel groupScheduleViewModel)
        {
            if (groupScheduleViewModel is null)
            {
                throw new ArgumentNullException(nameof(groupScheduleViewModel), $"{nameof(groupScheduleViewModel)} parameter cannot be null.");
            }

            return View(groupScheduleViewModel);
        }
    }
}
