using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Services;

public class ScoresRepository : IScoresRepository
{
    protected readonly ApplicationDbContext _context;

    public ScoresRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Score> Scores => _context.Scores
        .Include(l => l.StudentId)
        .Include(l => l.Subject)
        .Include(l => l.Type)
        .Include(l => l.Teacher)
        .Include(l => l.Value);

    public bool IsExists(Score score)
    {
        return this.IsExists(score.StudentId, score.Date, score.Number, score.Subgroup);
    }
    public bool IsExists(int studentId, DateTime date, int number, int subgroup)
    {
        return _context.Scores.Any(s =>
            s.StudentId == studentId &&
            s.Date == date &&
            s.Number == number &&
            s.Subgroup == subgroup);
    }

    public bool Create(Score score)
    {
        if (this.IsExists(score))
        {
            return false;
        }

        _context.Scores.Add(score);
        _context.SaveChanges();
        return true;
    }

    public Score? Get(int studentId, DateTime date, int number, int subgroup)
    {
        return _context.Scores.FirstOrDefault(s =>
            s.StudentId == studentId &&
            s.Date == date &&
            s.Number == number &&
            s.Subgroup == subgroup);
    }

    public bool Update(Score score)
    {
        var storedScore = this.Get(score.StudentId, score.Date, score.Number, score.Subgroup);

        if (storedScore == null)
        {
            return false;
        }

        if (storedScore.Subject.Name != score.Subject.Name)
        {
            storedScore.Subject = score.Subject;
        }

        if (storedScore.Type.Name != score.Type.Name)
        {
            storedScore.Type = score.Type;
        }

        if (storedScore.Teacher.Name != score.Teacher.Name)
        {
            storedScore.Teacher = score.Teacher;
        }

        if (storedScore.Value.Name != score.Value.Name)
        {
            storedScore.Value = score.Value;
        }

        _context.Scores.Update(storedScore);
        _context.SaveChanges();
        return true;
    }

    public bool Delete(Score score)
    {
        return this.Delete(score.StudentId, score.Date, score.Number, score.Subgroup);
    }
    public bool Delete(int studentId, DateTime date, int number, int subgroup)
    {
        var score = this.Get(studentId, date, number, subgroup);

        if (score == null)
        {
            return false;
        }

        _context.Scores.Remove(score);
        _context.SaveChanges();
        return true;
    }
}