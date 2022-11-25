using E_Journal.SchedulesApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Journal.SchedulesApi.Controllers
{
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ISchedulesRepository _repository;

        public SchedulesController(ISchedulesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("api/[controller]/groups/{name}/", Name = nameof(GetGroup))]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetGroup(string name, [FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
        {
            var query = _repository.Lessons
                .Where(l => l.GroupName == name);

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

            return Ok(query);
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
                .GroupBy(l => l.GroupName)
                .ToDictionary(g => g.Key, g => g.ToArray());

            return new JsonResult(result);
        }

        [HttpGet("api/[controller]/teathers/{name}/", Name = nameof(GetTeather))]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTeather(string name, [FromQuery] DateTime startDate = default, [FromQuery] DateTime endDate = default)
        {
            var query = _repository.Lessons
                .Where(l => l.TeatherName == name);

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

            return Ok(query);
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
                .GroupBy(l => l.TeatherName)
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

            var result = query.AsEnumerable();

            return new JsonResult(result);
        }
    }
}
