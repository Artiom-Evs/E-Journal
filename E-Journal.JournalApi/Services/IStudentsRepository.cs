using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public interface IStudentsRepository
{
    IQueryable<Student> Students { get; }

    ValueTask<bool> IsExistsAsync(int id);
    ValueTask<bool> IsExistsAsync(string name);

    ValueTask<bool> CreateAsync(Student student);
    ValueTask<Student?> GetAsync(int id);
    ValueTask<bool> UpdateAsync(Student student);
    ValueTask<bool> DeleteAsync(int id);
}
