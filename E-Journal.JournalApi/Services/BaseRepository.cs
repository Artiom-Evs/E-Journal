using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace E_Journal.JournalApi.Services;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseModel, new()
{
    protected readonly ApplicationDbContext _context;
    
    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual IQueryable<T> Items => _context.Set<T>().AsQueryable<T>();

    public virtual async ValueTask<bool> IsExistsAsync(int id)
    {
        return await _context.Set<T>().AnyAsync(item => item.Id == id);
    }
    
    public virtual async ValueTask<T?> GetAsync(int id)
    {
        return await this.Items.FirstOrDefaultAsync(item => item.Id == id);
    }

    public virtual async ValueTask<T?> CreateAsync(T item)
    {
        if (await this.IsExistsAsync(item.Id))
        {
            return null;
        }

        await _context.Set<T>().AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }
    public virtual async ValueTask<T?> UpdateAsync(T item)
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
    public virtual async ValueTask<T?> DeleteAsync(int id)
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
