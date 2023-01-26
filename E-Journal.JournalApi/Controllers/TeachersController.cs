using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly IBaseRepository<Teacher> _repostory;

    public TeachersController(IBaseRepository<Teacher> repostory)
    {
        _repostory = repostory;
    }

    // GET: api/Teachers
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Teacher[]))]
    public async Task<ActionResult<IEnumerable<Teacher>>> GetAsync()
    {
        return await _repostory.Items.ToListAsync();
    }

    // GET api/Teachers/5
    [ActionName(nameof(GetAsync))]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Teacher))]
    public async Task<IActionResult> GetAsync(int id)
    {
        Teacher? teacher = await _repostory.GetAsync(id);

        if (teacher == null)
        {
            return NotFound();
        }

        return Ok(teacher);
    }

    // POST api/Teachers
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Teacher))]
    public async Task<IActionResult> PostAsync([FromBody] Teacher teacher)
    {
        if (teacher.Id != 0 || string.IsNullOrWhiteSpace(teacher.Name))
        {
            return BadRequest();
        }

        var createdTeacher = await _repostory.CreateAsync(teacher);

        return CreatedAtAction(nameof(GetAsync), new { createdTeacher.Id }, createdTeacher);
    }

    // PUT api/Teachers/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Teacher))]
    public async Task<IActionResult> PutAsync(int id, [FromBody] Teacher teacher)
    {
        if (id != teacher.Id)
        {
            return BadRequest();
        }

        var updatedTeacher = await _repostory.UpdateAsync(teacher);

        if (updatedTeacher == null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetAsync), new { updatedTeacher.Id }, updatedTeacher);
    }

    // DELETE api/Teachers/5
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
