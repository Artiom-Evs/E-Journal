
using E_Journal.Shared;

namespace E_Journal.Infrastructure
{
    public interface IJournalRepository : IDisposable
    {
        IQueryable<Group> Groups { get; }
        IQueryable<Student> Students { get; }
        IQueryable<Teacher> Teachers { get; }
        IQueryable<Discipline> Disciplines { get; }
        IQueryable<TrainingSession> TrainingSessions { get; }
        IQueryable<Timetable> Timetables { get; }

        void Add<T>(T item) where T : class;
        void Update<T>(T item) where T : class;
        void Delete<T>(int id) where T : class;
        T? Get<T>(int id) where T : class;

        ValueTask AddAsync<T>(T item) where T : class;
        ValueTask UpdateAsync<T>(T item) where T : class;
        ValueTask DeleteAsync<T>(int id) where T : class;
        ValueTask<T?> GetAsync<T>(int id) where T : class;

        Group GetGroup(int id);
        Student GetStudent(int id);
        Teacher GetTeacher(int id);
        Discipline GetDiscipline(int id);
        TrainingSession GetTrainingSession(int id);
        StudentStatus GetStudentStatus(int id);
        Timetable GetTimetable(int id);

        Task<Group> GetGroupAsync(int id);
        Task<Student> GetStudentAsync(int id);
        Task<Teacher> GetTeacherAsync(int id);
        Task<Discipline> GetDisciplineAsync(int id);
        Task<TrainingSession> GetTrainingSessionAsync(int id);
        Task<StudentStatus> GetStudentStatusAsync(int id);
        Task<Timetable> GetTimetableAsync(int id);
        Task ClearDatabase();
    }
}
