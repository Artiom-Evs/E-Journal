using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Services;

public class StudentsRepository : BaseRepository<Student>
{
    public StudentsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override IQueryable<Student> Items => _context.Students
        .Include(t => t.Group)
        .AsQueryable();

    public override async ValueTask<Student?> CreateAsync(Student item)
    {
        if (item.Id != 0)
        {
            return null;
        }

        try
        {
            item.Group = await _context.Groups.FirstAsync(g => g.Id == item.GroupId);
        }
        catch
        {
            return null;
        }

        await _context.Students.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public override async ValueTask<Student?> UpdateAsync(Student item)
    {
        if (!await this.IsExistsAsync(item.Id))
        {
            return null;
        }

        if (!await _context.Groups.AnyAsync(g => g.Id == item.GroupId))
        {
            return null;
        }

        item.Group = await _context.Groups.FirstAsync(g => g.Id == item.GroupId);

        _context.Attach(item);
        _context.Entry(item).State = EntityState.Modified;
        _context.SaveChanges();

        return item;
    }
}