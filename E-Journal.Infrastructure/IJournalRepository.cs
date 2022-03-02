
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

        ValueTask Add<T>(T item) where T : class;
        ValueTask Update<T>(T item) where T : class;
        ValueTask Delete<T>(int id) where T : class;
        ValueTask<T?> Get<T>(int id) where T : class;

        Task<Group> GetGroup(int id);
        Task<Student> GetStudent(int id);
        Task<Teacher> GetTeacher(int id);
        Task<Discipline> GetDiscipline(int id);
        Task<TrainingSession> GetTrainingSession(int id);
        Task<StudentStatus> GetStudentStatus(int id);
        Task<Timetable> GetTimetable(int id);
        Task ClearDatabase();
    }
}
