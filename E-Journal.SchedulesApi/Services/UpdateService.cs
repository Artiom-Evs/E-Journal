using E_Journal.SchedulesApi.Models;
using E_Journal.SchedulesApi.Infrastructure;
using System.Diagnostics.Contracts;

namespace E_Journal.SchedulesApi.Services;

public class UpdateService : IUpdateService
{
    private readonly ILogger<UpdateService> _logger;
    private readonly ILessonsRepository _repository;
    private readonly IBaseRepository<Group> _groups;
    private readonly IBaseRepository<Subject> _subjects;
    private readonly IBaseRepository<Models.Type> _types;
    private readonly IBaseRepository<Teacher> _teachers;
    private readonly IBaseRepository<Room> _rooms;

    public UpdateService(ILogger<UpdateService> logger, ILessonsRepository repository, IBaseRepository<Group> groups, IBaseRepository<Subject> subjects, IBaseRepository<Models.Type> types, IBaseRepository<Teacher> teachers, IBaseRepository<Room> rooms)
    {
        _logger = logger;
        _repository = repository;
        _groups = groups;
        _subjects = subjects;
        _types = types;
        _teachers = teachers;
        _rooms = rooms;
    }

    /// <summary>
    /// Update schedules in the database. If data in actual state, it does nothing. 
    /// </summary>
    /// <param name="parsedLessons">Dictionary of parsed lessons with group names as keys.</param>
    public Task<bool> UpdateAsync(IEnumerable<Parser.Models.Lesson> parsedLessons)
    {
        try
        {
            foreach (var lessonsByGroupNames in parsedLessons.GroupBy(l => l.GroupName))
            {
                foreach (var lessonsByDates in lessonsByGroupNames.GroupBy(l => l.Date))
                {
                    UpdateDaylyLessons(lessonsByGroupNames.Key, lessonsByDates.Key, lessonsByDates);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occured while schedules update.");
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private void UpdateDaylyLessons(string groupName, DateTime date, IEnumerable<Parser.Models.Lesson> parsedLessons)
    {
        var group = _groups.GetOrCreate(groupName);

        var newLessons = parsedLessons.Select(pl => BuildLesson(pl));

        var storedLessons = _repository.Lessons
            .Where(l => l.GroupId == group.Id && l.Date == date)
            .AsEnumerable();

        var isEqual = Enumerable.SequenceEqual(newLessons, storedLessons, new LessonEqualityComparer());

        if (!isEqual)
        {
            var outdated = storedLessons.Except(newLessons, new LessonEqualityComparer());
            var updated = newLessons.Except(storedLessons, new LessonEqualityComparer());

            var forDeleting = outdated.Except(updated, new LessonPrimaryKeyEqualityComparer());
            var forAdding = updated.Except(outdated, new LessonPrimaryKeyEqualityComparer());
            var forUpdating = updated.Intersect(outdated, new LessonPrimaryKeyEqualityComparer());

            foreach (var item in forUpdating)
            {
                _repository.Update(item);
            }

            foreach (var item in forDeleting)
            {
                _repository.Delete(item);
            }

            foreach (var item in forAdding)
            {
                _repository.Create(item);
            }
        }
    }

    private Lesson BuildLesson(Parser.Models.Lesson parsedLesson)
    {
        var group = _groups.GetOrCreate(parsedLesson.GroupName);
        var subject = _subjects.GetOrCreate(parsedLesson.Title);
        var type = _types.GetOrCreate(parsedLesson.Type);
        var teacher = _teachers.GetOrCreate(parsedLesson.TeatherName);
        var room = _rooms.GetOrCreate(parsedLesson.Room);

        return new Lesson(subject, type, teacher, group, room, parsedLesson.Number, parsedLesson.Subgroup, parsedLesson.Date)
        {
            SubjectId = subject.Id,
            TypeId = type.Id,
            TeacherId = teacher.Id,
            GroupId = group.Id,
            RoomId = room.Id
        };
    }
}
