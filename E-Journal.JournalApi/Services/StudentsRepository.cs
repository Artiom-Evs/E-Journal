using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Services;

public class StudentsRepository : IStudentsRepository
{
    private readonly ApplicationDbContext _context;

    public StudentsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Student> Students => _context.Students
        .Include(s => s.Group);
    
    public async ValueTask<bool> IsExistsAsync(int id)
    {
        return await _context.Students.AnyAsync(s => s.Id == id);
    }
    public async ValueTask<bool> IsExistsAsync(string name)
    {
        return await _context.Students.AnyAsync(s => s.Name == name);
    }

    public async ValueTask<bool> CreateAsync(Student student)
    {
        if (await this.IsExistsAsync(student.Id))
        {
            return false;
        }

        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == student.Group.Name);

        if (group != null)
        {
            student.Group = group;
        }

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
        return true;
    }

    public async ValueTask<Student?> GetAsync(int id)
    {
        return await this.Students.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async ValueTask<bool> UpdateAsync(Student student)
    {
        if (!await this.IsExistsAsync(student.Id))
        {
            return false;
        }

        _context.Students.Attach(student);
        _context.Entry(student).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return true;
    }

    public async ValueTask<bool> DeleteAsync(int id)
    {
        var student = await this.GetAsync(id);

        if (student == null)
        {
            return false;
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}
