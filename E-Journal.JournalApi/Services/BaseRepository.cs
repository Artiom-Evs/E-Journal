using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Services;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel, new()
{
    protected readonly ApplicationDbContext _context;
    
    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Items => _context.Set<T>().AsQueryable<T>();

    public bool IsExists(int id)
    {
        return _context.Set<T>().Any(item => item.Id == id);
    }
    public bool IsExists(string name)
    {
        return _context.Set<T>().Any(item => item.Name == name);
    }

    public T? Get(int id)
    {
        return _context.Set<T>().FirstOrDefault(item => item.Id == id);
    }
    public T? Get(string name)
    {
        return _context.Set<T>().FirstOrDefault(item => item.Name == name);
    }

    public T GetOrCreate(string name)
    {
        var item = this.Get(name);

        if (item == null)
        {
            item = _context.Set<T>().Add(new T() { Name = name }).Entity;
            _context.SaveChanges();
        }

        return item;
    }

    public T? Create(string name)
    {
        if (this.IsExists(name))
        {
            return null;
        }

        var item = _context.Set<T>().Add(new T() { Name = name }).Entity;
        _context.SaveChanges();
        return item;
    }
    public T? Update(T item)
    {
        if (!this.IsExists(item.Id))
        {
            return null;
        }

        _context.Set<T>().Attach(item);
        item = _context.Set<T>().Update(item).Entity;
        _context.SaveChanges();
        return item;
    }
    public T? Delete(int id)
    {
        var item = this.Get(id);

        if (item == null)
        {
            return null;
        }

        _context.Set<T>().Remove(item);
        _context.SaveChanges();
        return item;
    }
}
