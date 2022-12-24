using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public interface IScoresRepository
{
    IQueryable<Score> Scores { get; }

    ValueTask<bool> IsExistsAsync(Score score);
    ValueTask<bool> IsExistsAsync(int studentId, DateTime date, int number);
    
    ValueTask<bool> CreateAsync(Score score);
    ValueTask<Score?> GetAsync(int studentId, DateTime date, int number);
    ValueTask<bool> UpdateAsync(int studentId, DateTime date, int number, Score score);

    ValueTask<bool> DeleteAsync(Score score);
    ValueTask<bool> DeleteAsync(int studentId, DateTime date, int number);
}
