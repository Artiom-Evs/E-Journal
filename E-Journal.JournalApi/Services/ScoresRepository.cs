using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Services;

public class ScoresRepository : IScoresRepository
{
    private readonly ApplicationDbContext _context;
    
    public ScoresRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Score> Scores => _context.Scores
        .Include(l => l.Student)
        .Include(l => l.Subject)
        .Include(l => l.Type)
        .Include(l => l.Teacher)
        .Include(l => l.Value);

    public async ValueTask<bool> IsExistsAsync(Score score)
    {
        return await this.IsExistsAsync(score.StudentId, score.Date, score.Number);
    }
    public async ValueTask<bool> IsExistsAsync(int studentId, DateTime date, int number)
    {
        return await _context.Scores.AnyAsync(s =>
            s.StudentId == studentId &&
            s.Date.Date == date &&
            s.Number == number);
    }

    public async ValueTask<bool> CreateAsync(Score score)
    {
        if (await this.IsExistsAsync(score))
        {
            return false;
        }

        await _context.Scores.AddAsync(score);
        await _context.SaveChangesAsync();
        return true;
    }

    public async ValueTask<Score?> GetAsync(int studentId, DateTime date, int number)
    {
        return await this.Scores.FirstOrDefaultAsync(s =>
            s.StudentId == studentId &&
            s.Date == date &&
            s.Number == number);
    }

    public async ValueTask<bool> UpdateAsync(int studentId, DateTime date, int number, Score score)
    {
        var storedScore = await this.GetAsync(studentId, date, number);

        if (storedScore == null)
        {
            return false;
        }

        // if the composite primary key will be changed
        // delete old score and add updated score as new 
        // we cannot change primary key or their part 
        if (studentId != score.StudentId || date != score.Date || number != score.Number)
        {
            await this.DeleteAsync(studentId, date, number);
            await _context.SaveChangesAsync();

            await this.CreateAsync(score);
            await _context.SaveChangesAsync();

            return true;
        }

        if (storedScore.SubjectId != score.Subject.Id)
        {
            storedScore.Subject = score.Subject;
        }

        if (storedScore.TypeId != score.Type.Id)
        {
            storedScore.Type = score.Type;
        }

        if (storedScore.TeacherId != score.Teacher.Id)
        {
            storedScore.Teacher = score.Teacher;
        }

        if (storedScore.ValueId != score.Value.Id)
        {
            storedScore.Value = score.Value;
        }

        _context.Entry(storedScore).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return true;
    }

    public async ValueTask<bool> DeleteAsync(Score score)
    {
        return await this.DeleteAsync(score.StudentId, score.Date, score.Number);
    }
    public async ValueTask<bool> DeleteAsync(int studentId, DateTime date, int number)
    {
        var score = await this.GetAsync(studentId, date, number);

        if (score == null)
        {
            return false;
        }

        _context.Scores.Remove(score);
        await _context.SaveChangesAsync();
        return true;
    }
}