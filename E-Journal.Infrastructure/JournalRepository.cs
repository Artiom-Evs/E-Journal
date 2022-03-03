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

        public void Add<T>(T item) where T : class
        {
            context.Add<T>(item);
            context.SaveChanges();
        }
        public void Update<T>(T item) where T : class
        {
            context.Update<T>(item);
            context.SaveChanges();
        }
        public void Delete<T>(int id) where T : class
        {
            T? item = context.Find<T>(id);

            if (item != null)
            {
                context.Remove<T>(item);
                context.SaveChanges();
            }
        }
        public T? Get<T>(int id) where T : class
        {
            return context.Find<T>(id);
        }


        public async ValueTask AddAsync<T>(T item) where T : class
        {
            context.Add<T>(item);
            await context.SaveChangesAsync();
        }
        public async ValueTask UpdateAsync<T>(T item) where T : class
        {
            context.Update<T>(item);
            await context.SaveChangesAsync();
            var str = context.ChangeTracker.DebugView.LongView;
        }
        public async ValueTask DeleteAsync<T>(int id) where T : class
        {
            T? item = await context.FindAsync<T>(id);

            if (item != null)
            {
                context.Remove<T>(item);
                await context.SaveChangesAsync();
            }
        }
        public ValueTask<T?> GetAsync<T>(int id) where T : class
        {
            return context.FindAsync<T>(id);
        }

        public Group GetGroup(int id) =>
            context.Groups
            .Include(g => g.Disciplines)
            .Include(g => g.Students)
            .First(g => g.Id == id);
        public Student GetStudent(int id) =>
            context.Students
            .Include(s => s.Group)
            .First(s => s.Id == id);
        public Teacher GetTeacher(int id) =>
            context.Teachers
            .Include(t => t.Disciplines)
            .First(t => t.Id == id);
        public Discipline GetDiscipline(int id) =>
            context.Disciplines
            .Include(d => d.Groups)
            .Include(d => d.Teachers)
            .Include(d => d.TrainingSessions)
            .First(s => s.Id == id);
        public TrainingSession GetTrainingSession(int id) =>
            context.TrainingSessions
            .Include(s => s.Discipline)
            .Include(s => s.Teacher)
            .Include(s => s.StudentStatuses)
            .First(s => s.Id == id);
        public StudentStatus GetStudentStatus(int id) =>
            context.StudentStatuses
            .Include(ss => ss.Student)
            .First(ss => ss.Id == id);
        public Timetable GetTimetable(int id) =>
            context.Timetables
            .Include(t => t.Group)
            .Include(t => t.TrainingSessions)
            .First(t => t.Id == id);

        public Task<Group> GetGroupAsync(int id) =>
            context.Groups
            .Include(g => g.Disciplines)
            .Include(g => g.Students)
            .FirstAsync(g => g.Id == id);
        public Task<Student> GetStudentAsync(int id) =>
            context.Students
            .Include(s => s.Group)
            .FirstAsync(s => s.Id == id);
        public Task<Teacher> GetTeacherAsync(int id) =>
            context.Teachers
            .Include(t => t.Disciplines)
            .FirstAsync(t => t.Id == id);
        public Task<Discipline> GetDisciplineAsync(int id) =>
            context.Disciplines
            .Include(d => d.Groups)
            .Include(d => d.Teachers)
            .Include(d => d.TrainingSessions)
            .FirstAsync(s => s.Id == id);
        public Task<TrainingSession> GetTrainingSessionAsync(int id) =>
            context.TrainingSessions
            .Include(s => s.Discipline)
            .Include(s => s.Teacher)
            .Include(s => s.StudentStatuses)
            .FirstAsync(s => s.Id == id);
        public Task<StudentStatus> GetStudentStatusAsync(int id) =>
            context.StudentStatuses
            .Include(ss => ss.Student)
            .FirstAsync(ss => ss.Id == id);
        public Task<Timetable> GetTimetableAsync(int id) =>
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
