using Microsoft.AspNetCore.Mvc;
using E_Journal.WebUI.Models.ViewModels;

namespace E_Journal.WebUI.Components
{
    public class TeacherLessonCellViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(TeacherLessonViewModel viewModel)
        {
            return View(viewModel);
        }
    }
}
