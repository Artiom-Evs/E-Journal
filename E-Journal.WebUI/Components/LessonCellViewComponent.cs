using Microsoft.AspNetCore.Mvc;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Components
{
    public class LessonCellViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<LessonViewModel> viewModels)
        {
            return View(viewModels.Any() ? viewModels.ToArray() : Array.Empty<LessonViewModel>());
        }
    }
}
