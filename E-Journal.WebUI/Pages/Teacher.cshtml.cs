using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;
using E_Journal.Infrastructure; 

namespace E_Journal.WebUI.Pages;

[Authorize(Roles = ApplicationRoles.Teacher)]
public class TeacherModel : PageModel
{
    private readonly ILogger<TeacherModel> _logger;
    private readonly UserManager<ApplicationUser> _userManager; 
    private readonly JournalDbContext _context;

    public LessonViewModel[] LessonsToday {get;set;} = Array.Empty<LessonViewModel>();
    public LessonViewModel[] LessonsTomorrow {get;set;} = Array.Empty<LessonViewModel>();

    public TeacherModel(
        ILogger<TeacherModel> logger,
        UserManager<ApplicationUser> userManager, 
        JournalDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }
    public async Task OnGetAsync()
    {
        ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

        SetLessonsToday(user.AssociatedId);
        SetLessonsTomorrow(user.AssociatedId);
    }

    private void SetLessonsToday(int teacherId)
    {
        DateTime dateToday = DateTime.Now.Date;

        var lessonsToday = _context.Lessons
            .Where(l => l.TeacherId == teacherId && l.Schedule.Date == dateToday)
            .Select(l => 
                new LessonViewModel{
                    DisciplineName = l.Discipline.Name, 
                    TeacherName = l.Teacher.Name, 
                    Room = l.Room, 
                    Number = l.Number, 
                    Subgroup = l.Subgroup
                })
            .ToArray();

        if (lessonsToday.Any())
        {
            LessonsToday = lessonsToday;
        }
    }

    private void SetLessonsTomorrow(int teacherId)
    {
        DateTime dateTomorrow = DateTime.Now.Date.AddDays(1);

        var lessonsTomorrow = _context.Lessons
            .Where(l => l.TeacherId == teacherId && l.Schedule.Date == dateTomorrow)
            .Select(l => 
                new LessonViewModel{
                    DisciplineName = l.Discipline.Name, 
                    TeacherName = l.Teacher.Name, 
                    Room = l.Room,
                    Number = l.Number,
                    Subgroup = l.Subgroup
                })
            .ToArray();

        if (lessonsTomorrow.Any())
        {
            LessonsTomorrow = lessonsTomorrow;
        }
    }
}