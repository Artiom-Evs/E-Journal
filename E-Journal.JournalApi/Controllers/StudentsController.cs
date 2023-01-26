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
public class StudentsController : ControllerBase
{
    private readonly IBaseRepository<Student> _repository;

    public StudentsController(IBaseRepository<Student> repository)
    {
        _repository = repository;
    }

    // GET: api/Students
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<Student>> GetAsync([FromQuery] int groupId, [FromQuery] string? name)
    {
        var query = _repository.Items;

        if (groupId != 0)
        {
            query = query.Where(s => s.GroupId == groupId);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(s => s.Name.Contains(name));
        }

        return await query.ToListAsync();
    }

    // GET: api/Students/5
    [ActionName(nameof(GetAsync))]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(int id)
    {
        var student = await _repository.GetAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    // PUT: api/Students/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PutAsync(int id, Student student)
    {
        if (id != student.Id)
        {
            return BadRequest();
        }

        var updatedStudent = await _repository.UpdateAsync(student);

        if (updatedStudent == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetAsync), new { updatedStudent.Id }, updatedStudent);
    }

    // POST: api/Students
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync(Student student)
    {
        if (student.Id != 0)
        {
            return BadRequest();
        }

        var createdStudent = await _repository.CreateAsync(student);

        if (createdStudent == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetAsync), new { createdStudent.Id }, createdStudent);
    }

    // DELETE: api/Students/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _repository.DeleteAsync(id);
        
        if (student == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}
