﻿using E_Journal.SchedulesApi.Models;
using E_Journal.SchedulesApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Journal.SchedulesApi.Controllers
{
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
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetGroup(string name, [FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
        {
            var group = _groups.Get(name);

            if (group == null)
            {
                return NotFound();
            }

            var query = _repository.Lessons
                .Where(l => l.GroupId == group.Id);

            if (!query.Any())
            {
                return NotFound();
            }

            if (startDate != default)
            {
                query = query.Where(l => l.Date >= startDate);
            }

            if (endDate != default)
            {
                query = query.Where(l => l.Date <= endDate);
            }

            return new JsonResult(query);
        }

        [HttpGet("api/[controller]/groups/all/", Name = nameof(GetAllGroups))]
        [Consumes("application/json")]
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
                .GroupBy(l => l.Group.Name)
                .ToDictionary(g => g.Key, g => g.ToArray());

            return new JsonResult(result);
        }

        [HttpGet("api/[controller]/teathers/{name}/", Name = nameof(GetTeather))]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTeather(string name, [FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
        {
            var teacher = _teachers.Get(name);

            if (teacher == null)
            {
                return NotFound();
            }

            var query = _repository.Lessons
                .Where(l => l.TeacherId == teacher.Id);

            if (!query.Any())
            {
                return NotFound();
            }

            if (startDate != default)
            {
                query = query.Where(l => l.Date >= startDate);
            }

            if (endDate != default)
            {
                query = query.Where(l => l.Date <= endDate);
            }

            return new JsonResult(query);
        }

        [HttpGet("api/[controller]/teathers/all/", Name = nameof(GetAllTeathers))]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllTeathers([FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
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
                .GroupBy(l => l.Teacher.Name)
                .ToDictionary(g => g.Key, g => g.ToArray());

            return new JsonResult(result);
        }

        [HttpGet("api/[controller]/all/", Name = nameof(GetAll))]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
            
            return new JsonResult(query);
        }
    }
}
