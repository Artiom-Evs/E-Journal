using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using E_Journal.WebUI.Models;
using E_Journal.WebUI.Models.ViewModels;
using E_Journal.Infrastructure;

namespace E_Journal.WebUI.Pages;

[Authorize(Roles = ApplicationRoles.Student)]
public class StudentModel : PageModel
{
    private readonly ILogger<StudentModel> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JournalDbContext _context;

    public LessonViewModel[] LessonsToday { get; set; } = Array.Empty<LessonViewModel>();
    public LessonViewModel[] LessonsTomorrow { get; set; } = Array.Empty<LessonViewModel>();

    public StudentModel(
        ILogger<StudentModel> logger,
        UserManager<ApplicationUser> userManager,
        JournalDbContext context)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

        SetLessonsToday(user.AssociatedId);
        SetLessonsTomorrow(user.AssociatedId);
    }

    private void SetLessonsToday(int groupId)
    {
        DateTime dateToday = DateTime.Now.Date;

        var scheduleId = _context.Schedules
            .SingleOrDefault(s => s.Date == dateToday && s.GroupId == groupId)
            ?.Id ?? 0;

        var lessonsToday = _context.Lessons
            .Where(l => l.ScheduleId == scheduleId)
            .Select(l =>
                new LessonViewModel
                {
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

    private void SetLessonsTomorrow(int groupId)
    {
        DateTime dateTomorrow = DateTime.Now.Date.AddDays(1);

        var scheduleId = _context.Schedules
            .SingleOrDefault(s => s.Date == dateTomorrow && s.GroupId == groupId)
            ?.Id ?? 0;

        var lessonsTomorrow = _context.Lessons
            .Where(l => l.ScheduleId == scheduleId)
            .Select(l =>
                new LessonViewModel
                {
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