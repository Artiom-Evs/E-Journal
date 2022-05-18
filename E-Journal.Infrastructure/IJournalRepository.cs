
using E_Journal.Shared;

namespace E_Journal.Infrastructure
{
    public interface IJournalRepository : IDisposable
    {
        IQueryable<Group> Groups { get; }
        IQueryable<Discipline> Disciplines { get; }
        IQueryable<Teacher> Teachers { get; }
        IQueryable<Student> Students { get; }
        IQueryable<Lesson> Lessons { get; }
        IQueryable<Score> Scores { get; }
        IQueryable<ScoreValue> ScoreValues { get; }

        void Add<T>(T item) where T : class;
        void Update<T>(T item) where T : class;
        void Delete<T>(int id) where T : class;
        T? Get<T>(int id) where T : class;

        ValueTask AddAsync<T>(T item) where T : class;
        ValueTask UpdateAsync<T>(T item) where T : class;
        ValueTask DeleteAsync<T>(int id) where T : class;
        ValueTask<T?> GetAsync<T>(int id) where T : class;

        Group GetGroup(int id);
        Discipline GetDiscipline(int id);
        Teacher GetTeacher(int id);
        Student GetStudent(int id);
        Lesson GetLesson(int id);
        Score GetScore(int id);
        ScoreValue GetScoreValue(int id);

        Task<Group> GetGroupAsync(int id);
        Task<Discipline> GetDisciplineAsync(int id);
        Task<Teacher> GetTeacherAsync(int id);
        Task<Student> GetStudentAsync(int id);
        Task<Lesson> GetLessonAsync(int id);
        Task<Score> GetScoreAsync(int id);
        Task<ScoreValue> GetScoreValueAsync(int id);

        Task ClearDatabaseAsync();
    }
}
