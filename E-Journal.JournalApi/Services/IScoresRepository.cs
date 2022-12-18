using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public interface IScoresRepository
{
    IQueryable<Score> Scores { get; }

    bool IsExists(Score score);
    bool IsExists(int studentId, DateTime date, int number, int subgroup);

    bool Create(Score score);
    Score? Get(int studentId, DateTime date, int number, int subgroup);
    bool Update(Score score);
    bool Delete(Score score);
    bool Delete(int studentId, DateTime date, int number, int subgroup);
}
