using Microsoft.AspNetCore.Mvc;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;
using E_Journal.Shared;
using E_Journal.Infrastructure;

namespace E_Journal.WebUI.Components;

public class TeacherScheduleViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(TeacherScheduleViewModel teacherScheduleViewModel)
    {
        if (teacherScheduleViewModel is null)
        {
            throw new ArgumentNullException(nameof(teacherScheduleViewModel), $"{nameof(teacherScheduleViewModel)} parameter cannot be null.");
        }

        return View(teacherScheduleViewModel);
    }
}
