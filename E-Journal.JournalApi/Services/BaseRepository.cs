using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace E_Journal.JournalApi.Services;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel, new()
{
    protected readonly ApplicationDbContext _context;
    
    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Items => _context.Set<T>().AsQueryable<T>();

    public async ValueTask<bool> IsExistsAsync(int id)
    {
        return await _context.Set<T>().AnyAsync(item => item.Id == id);
    }
    public async ValueTask<bool> IsExistsAsync(string name)
    {
        return await _context.Set<T>().AnyAsync(item => item.Name == name);
    }

    public async ValueTask<T?> GetAsync(int id)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(item => item.Id == id);
    }
    public async ValueTask<T?> GetAsync(string name)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(item => item.Name == name);
    }

    public async ValueTask<T> GetOrCreateAsync(string name)
    {
        var item = await this.GetAsync(name);

        if (item == null)
        {
            item = (await _context.Set<T>().AddAsync(new T() { Name = name })).Entity;
            await _context.SaveChangesAsync();
        }

        return item;
    }

    public async ValueTask<T?> CreateAsync(string name)
    {
        if (await this.IsExistsAsync(name))
        {
            return null;
        }

        var item = (await _context.Set<T>().AddAsync(new T() { Name = name })).Entity;
        await _context.SaveChangesAsync();
        return item;
    }
    public async ValueTask<T?> UpdateAsync(T item)
    {
        if (!await this.IsExistsAsync(item.Id))
        {
            return null;
        }

        _context.Set<T>().Attach(item);
        item = _context.Set<T>().Update(item).Entity;
        await _context.SaveChangesAsync();
        return item;
    }
    public async ValueTask<T?> DeleteAsync(int id)
    {
        var item = await this.GetAsync(id);

        if (item == null)
        {
            return null;
        }

        _context.Set<T>().Remove(item);
        await _context.SaveChangesAsync();
        return item;
    }
}
