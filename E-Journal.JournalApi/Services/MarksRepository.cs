using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Services;

public class MarksRepository : BaseRepository<Mark>
{
    public MarksRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override IQueryable<Mark> Items => _context.Marks
        .Include(t => t.Training)
        .Include(t => t.Student)
        .Include(t => t.Value)
        .AsQueryable();

    public override async ValueTask<Mark?> CreateAsync(Mark item)
    {
        if (item.Id != 0)
        {
            return null;
        }

        try
        {
            item.Training = await _context.Trainings.FirstAsync(t => t.Id == item.TrainingId);
            item.Student = await _context.Students.FirstAsync(s => s.Id == item.StudentId);
            item.Value = await _context.MarkValues.FirstAsync(v => v.Id == item.ValueId);
        }
        catch
        {
            return null;
        }

        await _context.Marks.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public override async ValueTask<Mark?> UpdateAsync(Mark item)
    {
        var storedMark = await this.GetAsync(item.Id);

        if (storedMark == null)
        {
            return null;
        }

        if (storedMark.TrainingId != item.TrainingId)
        {
            if (!await _context.Trainings.AnyAsync(t => t.Id == item.TrainingId)) return null;
            storedMark.TrainingId = item.TrainingId;
            _context.Entry(storedMark).Property(m => m.TrainingId).IsModified = true;
        }

        if (storedMark.StudentId != item.StudentId)
        {
            if (!await _context.Students.AnyAsync(s => s.Id == item.StudentId)) return null;
            storedMark.StudentId = item.StudentId;
            _context.Entry(storedMark).Property(m => m.StudentId).IsModified = true;
        }

        if (storedMark.ValueId != item.ValueId)
        {
            if (!await _context.MarkValues.AnyAsync(v => v.Id == item.ValueId)) return null;
            storedMark.ValueId = item.ValueId;
            _context.Entry(storedMark).Property(m => m.ValueId).IsModified = true;
        }

        await _context.SaveChangesAsync();
        return await this.Items.FirstAsync(m => m.Id == item.Id);
    }

}
