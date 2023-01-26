using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using Microsoft.Extensions.Hosting;

namespace E_Journal.JournalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarksController : ControllerBase
{
    private readonly IBaseRepository<Mark> _repository;

    public MarksController(IBaseRepository<Mark> repository)
    {
        _repository = repository;
    }

    // GET: api/Marks
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Mark[]))]
    public async Task<IEnumerable<Mark>> GetAsync([FromQuery] int trainingId, [FromQuery] int studentId, [FromQuery] int valueId)
    {
        var query = _repository.Items;

        if (trainingId != 0)
        {
            query = query.Where(m => m.TrainingId == trainingId);
        }

        if (studentId != 0)
        {
            query = query.Where(m => m.StudentId == studentId);
        }

        if (valueId != 0)
        {
            query = query.Where(m => m.ValueId == valueId);
        }

        return await query.ToListAsync();
    }

    // GET: api/Marks/5
    [ActionName(nameof(GetAsync))]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Mark))]
    public async Task<IActionResult> GetAsync(int id)
    {
        var mark = await _repository.GetAsync(id);

        if (mark == null)
        {
            return NotFound();
        }

        return Ok(mark);
    }

    // PUT: api/Marks/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Mark))]
    public async Task<IActionResult> PutAsync(int id, Mark mark)
    {
        if (id != mark.Id)
        {
            return BadRequest();
        }

        var updatedMark = await _repository.UpdateAsync(mark);

        if (updatedMark == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetAsync), new { updatedMark.Id }, updatedMark);
    }

    // POST: api/Marks
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Mark))]
    public async Task<IActionResult> PostAsync(Mark mark)
    {
        if (mark.Id != 0)
        {
            return BadRequest();
        }

        var createdMark = await _repository.CreateAsync(mark);

        if (createdMark == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetAsync), new { createdMark.Id }, createdMark);
    }

    // DELETE: api/Marks/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMark(int id)
    {
        var mark = await _repository.DeleteAsync(id);

        if (mark == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}
