using E_Journal.Shared;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.Infrastructure
{
    public class JournalRepository : IJournalRepository
    {
        private readonly JournalDbContext context;

        public JournalRepository(JournalDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Group> Groups => context.Groups;
        public IQueryable<Student> Students => context.Students;
        public IQueryable<Teacher> Teachers => context.Teachers;
        public IQueryable<Discipline> Disciplines => context.Disciplines;
        public IQueryable<TrainingSession> TrainingSessions => context.TrainingSessions;
        public IQueryable<StudentStatus> StudentStatuses => context.StudentStatuses;
        public IQueryable<Timetable> Timetables => context.Timetables;

        public async ValueTask Add<T>(T item) where T : class
        {
            context.Add<T>(item);
            await context.SaveChangesAsync();
        }
        public async ValueTask Update<T>(T item) where T : class
        {
            context.Update<T>(item);
            await context.SaveChangesAsync();
            var str = context.ChangeTracker.DebugView.LongView;
        }
        public async ValueTask Delete<T>(int id) where T : class
        {
            T? item = await context.FindAsync<T>(id);

            if (item != null)
            {
                context.Remove<T>(item);
                await context.SaveChangesAsync();
            }
        }
        public ValueTask<T?> Get<T>(int id) where T : class
        {
            return context.FindAsync<T>(id);
        }

        public Task<Group> GetGroup(int id) =>
            context.Groups
            .Include(g => g.Disciplines)
            .Include(g => g.Students)
            .FirstAsync(g => g.Id == id);

        public Task<Student> GetStudent(int id) =>
            context.Students
            .Include(s => s.Group)
            .FirstAsync(s => s.Id == id);

        public Task<Teacher> GetTeacher(int id) =>
            context.Teachers
            .Include(t => t.Disciplines)
            .FirstAsync(t => t.Id == id);

        public Task<Discipline> GetDiscipline(int id) =>
            context.Disciplines
            .Include(d => d.Groups)
            .Include(d => d.Teachers)
            .Include(d => d.TrainingSessions)
            .FirstAsync(s => s.Id == id);

        public Task<TrainingSession> GetTrainingSession(int id) =>
            context.TrainingSessions
            .Include(s => s.Discipline)
            .Include(s => s.Teacher)
            .Include(s => s.StudentStatuses)
            .FirstAsync(s => s.Id == id);

        public Task<StudentStatus> GetStudentStatus(int id) =>
            context.StudentStatuses
            .Include(ss => ss.Student)
            .FirstAsync(ss => ss.Id == id);

        public Task<Timetable> GetTimetable(int id) =>
            context.Timetables
            .Include(t => t.Group)
            .Include(t => t.TrainingSessions)
            .FirstAsync(t => t.Id == id);

        public async Task ClearDatabase()
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
