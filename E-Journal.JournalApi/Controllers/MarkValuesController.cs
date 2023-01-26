using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarkValuesController : ControllerBase
{
    private readonly IBaseRepository<MarkValue> _repostory;

    public MarkValuesController(IBaseRepository<MarkValue> repostory)
    {
        _repostory = repostory;
    }

    // GET: api/MarkValues
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarkValue[]))]
    public async Task<ActionResult<IEnumerable<MarkValue>>> GetAsync()
    {
        return await _repostory.Items.ToListAsync();
    }

    // GET api/MarkValues/5
    [ActionName(nameof(GetAsync))]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarkValue))]
    public async Task<IActionResult> GetAsync(int id)
    {
        var suject = await _repostory.GetAsync(id);

        if (suject == null)
        {
            return NotFound();
        }

        return Ok(suject);
    }

    // POST api/MarkValues
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MarkValue))]
    public async Task<IActionResult> PostAsync([FromBody] MarkValue markValue)
    {
        if (markValue.Id != 0 || string.IsNullOrWhiteSpace(markValue.Name))
        {
            return BadRequest();
        }

        var createdMarkValue = await _repostory.CreateAsync(markValue);

        return CreatedAtAction(nameof(GetAsync), new { createdMarkValue.Id }, createdMarkValue);
    }

    // PUT api/MarkValues/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MarkValue))]
    public async Task<IActionResult> PutAsync(int id, [FromBody] MarkValue markValue)
    {
        if (id != markValue.Id)
        {
            return BadRequest();
        }

        var updatedMarkValue = await _repostory.UpdateAsync(markValue);

        if (updatedMarkValue == null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetAsync), new { updatedMarkValue.Id }, updatedMarkValue);
    }

    // DELETE api/MarkValues/5
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
