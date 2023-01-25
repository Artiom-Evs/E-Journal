using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly IBaseRepository<Subject> _repostory;

    public SubjectsController(IBaseRepository<Subject> repostory)
    {
        _repostory = repostory;
    }

    // GET: api/Subjects
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Subject[]))]
    public async Task<ActionResult<IEnumerable<Subject>>> GetAsync()
    {
        return await _repostory.Items.ToListAsync();
    }

    // GET api/Subjects/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Subject))]
    public async Task<IActionResult> GetAsync(int id)
    {
        var suject = await _repostory.GetAsync(id);

        if (suject == null)
        {
            return NotFound();
        }

        return Ok(suject);
    }

    // POST api/Subjects
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Subject))]
    public async Task<IActionResult> PostAsync([FromBody] Subject subject)
    {
        if (subject.Id != 0 || string.IsNullOrWhiteSpace(subject.Name))
        {
            return BadRequest();
        }

        var storedSubject = await _repostory.CreateAsync(subject);

        return Created($"/api/subjects/{storedSubject.Id}", storedSubject);
    }

    // PUT api/Subjects/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PutAsync(int id, [FromBody] Subject subject)
    {
        if (id != subject.Id)
        {
            return BadRequest();
        }

        var updatedSubject = await _repostory.UpdateAsync(subject);

        if (updatedSubject == null)
        {
            return NotFound();
        }

        return Created($"/api/subjects/{updatedSubject.Id}", updatedSubject);
    }

    // DELETE api/Subjects/5
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
