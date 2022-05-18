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
        public IQueryable<Discipline> Disciplines => context.Disciplines;
        public IQueryable<Teacher> Teachers => context.Teachers;
        public IQueryable<Student> Students => context.Students;
        public IQueryable<Lesson> Lessons => context.Lessons;
        public IQueryable<Score> Scores => context.Scores;
        public IQueryable<ScoreValue> ScoreValues => context.ScoreValues;

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
            .First(g => g.Id == id);
        public Discipline GetDiscipline(int id) =>
            context.Disciplines
            .First(s => s.Id == id);
        public Teacher GetTeacher(int id) =>
            context.Teachers
            .First(t => t.Id == id);
        public Student GetStudent(int id) =>
            context.Students
            .Include(s => s.Group)
            .First(s => s.Id == id);
        public Lesson GetLesson(int id) =>
            context.Lessons
            .Include(l => l.Discipline)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .First(l => l.Id == id);
        public Score GetScore(int id) =>
            context.Scores
            .Include(s => s.Student)
            .Include(s => s.Lesson)
            .First(s => s.Id == id);
        public ScoreValue GetScoreValue(int id) =>
            context.ScoreValues
            .First(v => v.Id == id);

        public Task<Group> GetGroupAsync(int id) =>
            context.Groups
            .FirstAsync(g => g.Id == id);
        public Task<Discipline> GetDisciplineAsync(int id) =>
            context.Disciplines
            .FirstAsync(s => s.Id == id);
        public Task<Teacher> GetTeacherAsync(int id) =>
            context.Teachers
            .FirstAsync(t => t.Id == id);
        public Task<Student> GetStudentAsync(int id) =>
            context.Students
            .Include(s => s.Group)
            .FirstAsync(s => s.Id == id);
        public Task<Lesson> GetLessonAsync(int id) =>
            context.Lessons
            .Include(l => l.Discipline)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .FirstAsync(l => l.Id == id);
        public Task<Score> GetScoreAsync(int id) =>
            context.Scores
            .Include(s => s.Student)
            .Include(s => s.Lesson)
            .FirstAsync(s => s.Id == id);
        public Task<ScoreValue> GetScoreValueAsync(int id) =>
            context.ScoreValues
            .FirstAsync(v => v.Id == id);

        public async Task ClearDatabaseAsync()
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
