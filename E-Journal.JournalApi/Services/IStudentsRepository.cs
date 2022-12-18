using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public interface IStudentsRepository
{
    IQueryable<Student> Students { get; }

    bool IsExists(int id);
    bool IsExists(string name);

    bool Create(Student student);
    Student? Get(int id);
    bool Update(Student student);
    bool Delete(int id);
}
