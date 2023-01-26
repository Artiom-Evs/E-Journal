using E_Journal.JournalApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace E_Journal.JournalApi.Services;

public class TrainingRepository : BaseRepository<Training>
{
    public TrainingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override IQueryable<Training> Items => _context.Trainings
        .Include(t => t.Group)
        .Include(t => t.Subject)
        .Include(t => t.Type)
        .Include(t => t.Teacher)
        .AsQueryable();

    public override async ValueTask<Training?> CreateAsync(Training item)
    {
        if (item.Id != 0)
        {
            return null;
        }

        try
        {
            item.Group = await _context.Groups.FirstAsync(g => g.Id == item.GroupId);
            item.Subject = await _context.Subjects.FirstAsync(s => s.Id == item.SubjectId);
            item.Type = await _context.TrainingTypes.FirstAsync(t => t.Id == item.TypeId);
            item.Teacher = await _context.Teachers.FirstAsync(t => t.Id == item.TeacherId);
        }
        catch
        {
            return null;
        }

        await _context.Trainings.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public override async ValueTask<Training?> UpdateAsync(Training item)
    {
        var storedTraining = await this.GetAsync(item.Id);

        if (storedTraining == null)
        {
            return null;
        }

        if (storedTraining.GroupId != item.GroupId)
        {
            if (!await _context.Groups.AnyAsync(g => g.Id == item.GroupId)) return null;
            storedTraining.GroupId = item.GroupId;
            _context.Entry(storedTraining).Property(t => t.GroupId).IsModified = true;
        }

        if (storedTraining.SubjectId != item.SubjectId)
        {
            if (!await _context.Subjects.AnyAsync(s => s.Id == item.SubjectId)) return null;
            storedTraining.SubjectId = item.SubjectId;
            _context.Entry(storedTraining).Property(t => t.SubjectId).IsModified = true;
        }

        if (storedTraining.TypeId != item.TypeId)
        {
            if (!await _context.TrainingTypes.AnyAsync(t => t.Id == item.TypeId)) return null;
            storedTraining.TypeId = item.TypeId;
            _context.Entry(storedTraining).Property(t => t.TypeId).IsModified = true;
        }

        if (storedTraining.TeacherId != item.TeacherId)
        {
            if (!await _context.Teachers.AnyAsync(t => t.Id == item.TeacherId)) return null;
            storedTraining.TeacherId = item.TeacherId;
            _context.Entry(storedTraining).Property(t => t.TeacherId).IsModified = true;
        }

        if (storedTraining.Date != item.Date)
        {
            storedTraining.Date = item.Date;
            _context.Entry(storedTraining).Property(t => t.Date).IsModified = true;
        }

        if (storedTraining.Number != item.Number)
        {
            storedTraining.Number = item.Number;
            _context.Entry(storedTraining).Property(t => t.Number).IsModified = true;
        }

        await _context.SaveChangesAsync();
        return await this.Items.FirstAsync(t => t.Id == item.Id);
    }
}
 