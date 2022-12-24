using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Journal.JournalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScoresController : ControllerBase
{
    private readonly IScoresRepository _repository;
    private readonly IStudentsRepository _students;
    private readonly IBaseRepository<Subject> _subjects;
    private readonly IBaseRepository<Models.Type> _types;
    private readonly IBaseRepository<Teacher> _teachers;
    private readonly IBaseRepository<ScoreValue> _scoreValues;

    public ScoresController(IScoresRepository repository, IStudentsRepository students, IBaseRepository<Subject> subjects, IBaseRepository<Models.Type> types, IBaseRepository<Teacher> teachers, IBaseRepository<ScoreValue> scoreValues)
    {
        _repository = repository;
        _students = students;
        _subjects = subjects;
        _types = types;
        _teachers = teachers;
        _scoreValues = scoreValues;
    }

    // GET: api/Scores
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ScoreIOModel[]))]
    public async Task<ActionResult<IEnumerable<ScoreIOModel>>> GetScores()
    {
        var scores = await _repository.Scores.ToListAsync();

        return scores.Select(s => ConvertToScoreIOModel(s)).ToList();
    }

    // GET: api/Scores/5/2022-12-21
    [HttpGet("{studentId}/{date}/{number}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ScoreIOModel))]
    public async Task<ActionResult<ScoreIOModel>> GetScore(int studentId, DateTime date, int number)
    {
        var score = await _repository.GetAsync(studentId, date.Date, number);

        if (score == null)
        {
            return NotFound();
        }

        return ConvertToScoreIOModel(score);
    }

    // POST: api/Scores
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ScoreIOModel))]
    public async Task<ActionResult<ScoreIOModel>> PostScore(ScoreIOModel score)
    {
        if (!await _students.IsExistsAsync(score.StudentId))
        {
            return BadRequest(new Dictionary<string, string>() { { "StudentId", "The student with the given ID does not exist." } });
        }

        if (!await _repository.CreateAsync(await ConvertToScoreAsync(score)))
        {
            return Conflict();
        }
        
        return CreatedAtAction("GetScore", new { studentId = score.StudentId, date = score.Date.Date, number = score.Number }, score);
    }

    // PUT: api/Scores/5/2022-12-21
    [HttpPut("{studentId}/{date}/{number}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PutScore(int studentId, DateTime date, int number, ScoreIOModel score)
    {
        if (!await _repository.IsExistsAsync(studentId, date.Date, number))
        {
            return NotFound();
        }

        // if the composite primary key will be changed
        // check the new composite primary key to avoid conflict
        if ((studentId !=  score.StudentId || date.Date != score.Date || number != score.Number) && 
            await _repository.IsExistsAsync(score.StudentId, score.Date, score.Number))
        {
            return Conflict();
        }

        if (!await _repository.UpdateAsync(studentId, date.Date, number, await ConvertToScoreAsync(score)))
        {
            return BadRequest();
        }

        return NoContent();
    }

    // DELETE: api/Scores/5
    [HttpDelete("{studentId}/{date}/{number}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteScore(int studentId, DateTime date, int number)
    {
        if (!await _repository.DeleteAsync(studentId, date, number))
        {
            return NotFound();
        }

        return NoContent();
    }

    private async ValueTask<Score> ConvertToScoreAsync(ScoreIOModel iOModel)
    {
        Score score = new()
        {
            Subject = await _subjects.GetOrCreateAsync(iOModel.Subject), 
            Type = await _types.GetOrCreateAsync(iOModel.Type), 
            Teacher = await _teachers.GetOrCreateAsync(iOModel.Teacher), 
            Value = await _scoreValues.GetOrCreateAsync(iOModel.Value),
        };

        score.Student = await _students.GetAsync(iOModel.StudentId);
        score.StudentId = score.Student.Id;
        score.SubjectId = score.Subject.Id;
        score.TypeId = score.Type.Id;
        score.TeacherId = score.Teacher.Id;
        score.ValueId = score.Value.Id;

        score.Date = iOModel.Date.Date;
        score.Number = iOModel.Number;
        
        return score;
    }

    private ScoreIOModel ConvertToScoreIOModel(Score score)
    {
        return new ScoreIOModel()
        {
            StudentId = score.StudentId != 0 ? score.StudentId : score.Student?.Id ?? 0, 
            Date = score.Date.Date, 
            Number = score.Number, 
            Subject = score.Subject.Name, 
            Type = score.Type.Name, 
            Teacher = score.Teacher.Name,
            Value = score.Value.Name
        };
    }
}
