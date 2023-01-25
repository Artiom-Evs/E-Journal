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
    private readonly IStudentsRepository _repository;
    private readonly IBaseRepository<Group> _groups;

    public StudentsController(IStudentsRepository repository, IBaseRepository<Group> groups)
    {
        _repository = repository;
        _groups = groups;
    }

    // GET: api/Students
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentIOModel[]))]
    public async Task<ActionResult<IEnumerable<StudentIOModel>>> GetStudents([FromQuery] int groupId = 0)
    {
        var query = _repository.Students;

        if (groupId != 0)
        {
            query = query.Where(s => s.GroupId == groupId);
        }

        var students = await query.ToListAsync();

        return students.Select(s => ConvertToStudentIOModel(s)).ToList();
    }

    // GET: api/Students/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentIOModel))]
    public async Task<ActionResult<StudentIOModel>> GetStudent(int id)
    {
        var student = await _repository.GetAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return ConvertToStudentIOModel(student);
    }

    // PUT: api/Students/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PutStudent(int id, StudentIOModel student)
    {
        if (id != student.Id)
        {
            return BadRequest();
        }

        if (!await _repository.UpdateAsync(await ConvertToStudentAsync(student)))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Students
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StudentIOModel))]
    public async Task<ActionResult<StudentIOModel>> PostStudent(StudentIOModel student)
    {
        if (student.Id != 0)
        {
            return BadRequest();
        }

        if (!await _repository.CreateAsync(await ConvertToStudentAsync(student)))
        {
            return Conflict();
        }

        var createdStudent = await _repository.Students.FirstAsync(s => s.Name == student.Name);

        return CreatedAtAction("GetStudent", new { id = createdStudent.Id }, ConvertToStudentIOModel(createdStudent));
    }

    // DELETE: api/Students/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        if (!await _repository.DeleteAsync(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    private StudentIOModel ConvertToStudentIOModel(Student student)
    {
        return new StudentIOModel()
        {
            Id = student.Id,
            Name = student.Name,
            Group = student.Group.Name, 
            GroupId= student.Group.Id
        };
    }

    private async Task<Student> ConvertToStudentAsync(StudentIOModel iOModel)
    {
        Student student = new()
        {
            Id = iOModel.Id,
            Name = iOModel.Name,
        };

        student.Group = await _groups.GetAsync(iOModel.GroupId) ?? await _groups.CreateAsync(new() { Name = iOModel.Group });
        student.GroupId = student.Group.Id;

        return student;
    }
}
