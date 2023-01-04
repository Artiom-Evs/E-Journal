using E_Journal.SchedulesApi.Models;
using E_Journal.SchedulesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Journal.SchedulesApi.Controllers;

[ApiController]
public class SchedulesController : ControllerBase
{
    private readonly ILessonsRepository _repository;
    private readonly IBaseRepository<Group> _groups;
    private readonly IBaseRepository<Teacher> _teachers;

    public SchedulesController(ILessonsRepository repository, IBaseRepository<Group> groups, IBaseRepository<Teacher> teachers)
    {
        _repository = repository;
        _groups = groups;
        _teachers = teachers;
    }

    [HttpGet("api/[controller]/groups/{name}/", Name = nameof(GetGroup))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OutputLessonModel[]))]
    public IActionResult GetGroup(string name, [FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
    {
        var group = _groups.Get(name);

        if (group == null)
        {
            return NotFound();
        }

        var query = _repository.Lessons
            .Where(l => l.GroupId == group.Id);

        if (startDate != default)
        {
            query = query.Where(l => l.Date >= startDate);
        }

        if (endDate != default)
        {
            query = query.Where(l => l.Date <= endDate);
        }

        var result = query.AsEnumerable()
            .Select(l => new OutputLessonModel(l));

        return new JsonResult(result);
    }

    [HttpGet("api/[controller]/groups/", Name = nameof(GetAllGroups))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllGroups([FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
    {
        var query = _repository.Lessons;

        if (startDate != default)
        {
            query = query.Where(l => l.Date >= startDate);
        }

        if (endDate != default)
        {
            query = query.Where(l => l.Date <= endDate);
        }

        var result = query.AsEnumerable()
            .Select(l => new OutputLessonModel(l))
            .GroupBy(l => l.Group)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return new JsonResult(result);
    }

    [HttpGet("api/[controller]/teachers/{name}/", Name = nameof(GetTeacher))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OutputLessonModel[]))]
    public IActionResult GetTeacher(string name, [FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
    {
        var teacher = _teachers.Get(name);

        if (teacher == null)
        {
            return NotFound();
        }

        var query = _repository.Lessons
            .Where(l => l.TeacherId == teacher.Id);

        if (startDate != default)
        {
            query = query.Where(l => l.Date >= startDate);
        }

        if (endDate != default)
        {
            query = query.Where(l => l.Date <= endDate);
        }

        var result = query.AsEnumerable()
            .Select(l => new OutputLessonModel(l));

        return new JsonResult(result);
    }

    [HttpGet("api/[controller]/teachers/", Name = nameof(GetAllTeachers))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OutputLessonModel[]))]
    public IActionResult GetAllTeachers([FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
    {
        var query = _repository.Lessons;

        if (startDate != default)
        {
            query = query.Where(l => l.Date >= startDate);
        }

        if (endDate != default)
        {
            query = query.Where(l => l.Date <= endDate);
        }

        var result = query.AsEnumerable()
            .Select(l => new OutputLessonModel(l))
            .GroupBy(l => l.Teacher)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return new JsonResult(result);
    }

    [HttpGet("api/[controller]/", Name = nameof(GetAll))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OutputLessonModel[]))]
    public IActionResult GetAll([FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
    {
        var query = _repository.Lessons;

        if (startDate != default)
        {
            query = query.Where(l => l.Date >= startDate);
        }

        if (endDate != default)
        {
            query = query.Where(l => l.Date <= endDate);
        }

        var result = query.AsEnumerable()
            .Select(l => new OutputLessonModel(l));

        return new JsonResult(result);
    }
}
