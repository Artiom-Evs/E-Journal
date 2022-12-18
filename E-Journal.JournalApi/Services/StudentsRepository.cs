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

    public bool IsExists(int id)
    {
        return _context.Students.Any(s => s.Id == id);
    }
    public bool IsExists(string name)
    {
        return _context.Students.Any(s => s.Name == name);
    }

    public bool Create(Student student)
    {
        if (this.IsExists(student.Id))
        {
            return false;
        }

        _context.Students.Add(student);
        _context.SaveChanges();
        return true;
    }

    public Student? Get(int id)
    {
        return this.Students.FirstOrDefault(s => s.Id == id);
    }

    public bool Update(Student student)
    {
        var storedStudent = this.Get(student.Id);

        if (storedStudent == null)
        {
            return false;
        }

        if (storedStudent.Name != student.Name)
        {
            storedStudent.Name = student.Name;
        }

        if (storedStudent.Group.Name != student.Group.Name)
        {
            storedStudent.Group = student.Group;
        }

        _context.Students.Update(storedStudent);
        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var student = this.Get(id);

        if (student == null)
        {
            return false;
        }

        _context.Students.Remove(student);
        _context.SaveChanges();
        return true;
    }
}
