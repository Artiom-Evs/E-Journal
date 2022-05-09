using Microsoft.AspNetCore.Mvc;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Components
{
    public class StudentLessonCellViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<StudentLessonViewModel> viewModels)
        {
            return View(viewModels.Any() ? viewModels.ToArray() : Array.Empty<StudentLessonViewModel>());
        }
    }
}
