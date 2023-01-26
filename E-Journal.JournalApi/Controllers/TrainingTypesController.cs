using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrainingTypesController : ControllerBase
{
    private readonly IBaseRepository<TrainingType> _repostory;

    public TrainingTypesController(IBaseRepository<TrainingType> repostory)
    {
        _repostory = repostory;
    }

    // GET: api/TrainingTypes
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrainingType[]))]
    public async Task<ActionResult<IEnumerable<TrainingType>>> GetAsync()
    {
        return await _repostory.Items.ToListAsync();
    }

    // GET api/TrainingTypes/5
    [ActionName(nameof(GetAsync))]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrainingType))]
    public async Task<IActionResult> GetAsync(int id)
    {
        TrainingType? trainingType = await _repostory.GetAsync(id);

        if (trainingType == null)
        {
            return NotFound();
        }

        return Ok(trainingType);
    }

    // POST api/TrainingTypes
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrainingType))]
    public async Task<IActionResult> PostAsync([FromBody] TrainingType trainingType)
    {
        if (trainingType.Id != 0 || string.IsNullOrWhiteSpace(trainingType.Name))
        {
            return BadRequest();
        }

        var createdTrainingType = await _repostory.CreateAsync(trainingType);

        return CreatedAtAction(nameof(GetAsync), new { createdTrainingType.Id }, createdTrainingType);
    }

    // PUT api/TrainingTypes/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrainingType))]
    public async Task<IActionResult> PutAsync(int id, [FromBody] TrainingType trainingType)
    {
        if (id != trainingType.Id)
        {
            return BadRequest();
        }

        var updatedTrainingType = await _repostory.UpdateAsync(trainingType);

        if (updatedTrainingType == null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetAsync), new { updatedTrainingType.Id }, updatedTrainingType);
    }

    // DELETE api/TrainingTypes/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _repostory.IsExistsAsync(id))
        {
            return NotFound();
        }

        await _repostory.DeleteAsync(id);

        return NoContent();
    }
}
