using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public interface IBaseRepository<T> where T : class, IBaseModel, new()
{
    IQueryable<T> Items { get; }

    bool IsExists(int id);
    bool IsExists(string name);

    T? Get(int id);
    T? Get(string name);
    T GetOrCreate(string name);
    T? Create(string name);
    T? Update(T item);
    T? Delete(int id);
}
