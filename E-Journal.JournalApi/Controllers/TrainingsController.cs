using E_Journal.JournalApi.Services;
using E_Journal.JournalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainingsController : ControllerBase
{
    private readonly IBaseRepository<Training> _repository;

    public TrainingsController(IBaseRepository<Training> repository)
    {
        _repository = repository;
    }



    // GET: api/Trainings
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Training[]))]
    public async Task<IEnumerable<Training>> GetAsync(
        int groupId, 
        int teacherId,
        int typeId, 
        DateTime startDate, 
        DateTime endDate)
    {
        var query = _repository.Items;

        if (groupId != 0)
        {
            query = query.Where(t => t.GroupId == groupId);
        }

        if (teacherId != 0)
        {
            query = query.Where(t => t.TeacherId == teacherId);
        }

        if (typeId != 0)
        {
            query = query.Where(t => t.TypeId == typeId);
        }

        if (startDate != default)
        {
            query = query.Where(t => t.Date >= startDate);
        }

        if (endDate != endDate)
        {
            query = query.Where(t => t.Date <= endDate);
        }

        return await query.ToListAsync();
    }

    // GET api/Trainings/5
    [ActionName(nameof(GetAsync))]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Training))]
    public async Task<IActionResult> GetAsync(int id)
    {
        var training = await _repository.GetAsync(id);

        if (training == null)
        {
            return NotFound();
        }

        return Ok(training);
    }

    // POST api/Trainings
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Training))]
    public async Task<IActionResult> PostAsync([FromBody] Training training)
    {
        if (training.Id != 0)
        {
            return BadRequest();
        }

        var createdTraining = await _repository.CreateAsync(training);

        if (createdTraining == null)
        {
            return BadRequest();
        }
        
        return CreatedAtAction(nameof(GetAsync), new { id = createdTraining.Id }, createdTraining);
    }

    // PUT api/<TrainingsController>/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Training))]
    public async Task<IActionResult> PutAsync(int id, [FromBody] Training training)
    {
        if (id != training.Id)
        {
            return BadRequest();
        }

        var updatedTraining = await _repository.UpdateAsync(training);

        if (updatedTraining == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetAsync), new { id = updatedTraining.Id }, updatedTraining);
    }

    // DELETE api/Trainings/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (await _repository.IsExistsAsync(id))
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);

        return NoContent();
    }
}
