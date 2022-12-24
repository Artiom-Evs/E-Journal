using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public interface IBaseRepository<T> where T : class, IBaseModel, new()
{
    IQueryable<T> Items { get; }

    ValueTask<bool> IsExistsAsync(int id);
    ValueTask<bool> IsExistsAsync(string name);

    ValueTask<T?> GetAsync(int id);
    ValueTask<T?> GetAsync(string name);
    ValueTask<T> GetOrCreateAsync(string name);
    ValueTask<T?> CreateAsync(string name);
    ValueTask<T?> UpdateAsync(T item);
    ValueTask<T?> DeleteAsync(int id);
}
