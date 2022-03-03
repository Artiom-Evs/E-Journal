using E_Journal.Shared;
using E_Journal.Parser;
using System.Globalization;

namespace E_Journal.Infrastructure
{
    public class TimetableBuilder
    {
        private readonly IJournalRepository repository;

        public TimetableBuilder(IJournalRepository repository)
        {
            this.repository = repository;
        }

        public async IAsyncEnumerable<Group> BuildAllGroups(ParseResult[] results)
        {
            foreach (var result in results)
            {
                yield return await BuildGroup(result);
            }
        }
        public async ValueTask<Group> BuildGroup(ParseResult result)
        {
            Group group = await GetOrCreateGroup(result.Name);
            Timetable timetable = new() { Group = group };
            group.Timetables.Add(timetable);

            if (result.DateRange is null)
            {
                return group;
            }

            string[] dates = result.DateRange.Split(" - ").ToArray();
            timetable.StartDate = DateTime.Parse(dates[0], new CultureInfo("ru-Ru"), DateTimeStyles.None);
            timetable.EndDate = DateTime.Parse(dates[1], new CultureInfo("ru-Ru"), DateTimeStyles.None);

            if (result.Days is null || result.Timetable is null)
            {
                return group;
            }

            foreach (var (First, Second) in result.Days.Zip(result.Timetable))
            {
                await foreach (var session in BuildDay(First, Second))
                {
                    session.Group = group;
                    Add<Discipline>(group.Disciplines, session.Discipline);
                    Add<Group>(session.Discipline.Groups, group);
                    timetable.TrainingSessions.Add(session);
                }
            }

            return group;
        }
        private async IAsyncEnumerable<TrainingSession> BuildDay(DateTime date, string[] day_sessions)
        {
            for (int i = 0; i < day_sessions.Length; i += 2)
            {
                if (day_sessions[i] == "") continue;

                await foreach (var session in BuildCell(day_sessions[i], day_sessions[i + 1], date, i / 2 + 1))
                {
                    yield return session;
                }
            }
        }
        private async IAsyncEnumerable<TrainingSession> BuildCell(string cell, string room, DateTime date, int number)
        {
            var distRows = cell.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split('\n');
            var roomRows = room.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split('\n');

            for (int row = 0; row < distRows.Length; row += 3)
            {
                var discipline = await GetOrCreateDiscipline(distRows[row]);
                var teacher = await GetOrCreateTeacher(distRows[row + 2]);

                Add<Teacher>(discipline.Teachers, teacher);
                Add<Discipline>(teacher.Disciplines, discipline);

                TrainingSession session = new()
                {
                    Date = date,
                    Discipline = discipline,
                    Teacher = teacher,
                    Room = (row / 3 < roomRows.Length) ? roomRows[row / 3] : roomRows[0],
                    Number = (byte)number
                };

                if (distRows.Length > 3)
                {
                    session.Subgroup = char.Parse((row / 3 + 1).ToString());
                }

                discipline.TrainingSessions.Add(session);
                yield return session;
            }
        }

        private async ValueTask<Group> GetOrCreateGroup(string group_name)
        {
            Group? group = repository.Groups.FirstOrDefault(g => g.Name == group_name);

            if (group == null)
            {
                group = new() { Name = group_name };
                await repository.AddAsync<Group>(group);
            }

            return group;
        }
        private async ValueTask<Discipline> GetOrCreateDiscipline(string discipline_name)
        {
            Discipline? discipline = repository.Disciplines.FirstOrDefault(d => d.Name == discipline_name);

            if (discipline == null)
            {
                discipline = new() { Name = discipline_name };
                await repository.AddAsync<Discipline>(discipline);
            }

            return discipline;
        }
        private async ValueTask<Teacher> GetOrCreateTeacher(string teacher_name)
        {
            Teacher? teacher = repository.Teachers.FirstOrDefault(t => t.Name == teacher_name);

            if (teacher == null)
            {
                teacher = new() { Name = teacher_name };
                await repository.AddAsync<Teacher>(teacher);
            }

            return teacher;
        }
        private static void Add<T>(ICollection<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
