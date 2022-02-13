using System.Linq;
using System.Collections.Generic;

using E_Journal.Parser;
using E_Journal.Shared;

namespace E_Journal.Builder
{
    public static class ObjectBuilder
    {
        private static (List<TrainingSession> Sessions, List<Discipline> Disciplines, List<Teacher> Teachers, List<Group> Groups) objectTree;

        public static (TrainingSession[]?, Discipline[]?, Teacher[]?, Group[]?) BuildAllGroups(ParseResult[] results)
        {
            objectTree = new(new(), new(), new(), new());
            
            foreach (var result in results)
            {
                BuildGroup(result);
            }

            var objects = objectTree;
            objectTree = new();

            return (objects.Sessions?.ToArray(), objects.Disciplines?.ToArray(), objects.Teachers?.ToArray(), objects.Groups?.ToArray());
        }

        public static void BuildGroup(ParseResult result)
        {
            Group group = new() { Name = result.Name };
            objectTree.Groups.Add(group);
            
            if (result.Days is null || result.Timetable is null)
            {
                return;
            }

            foreach (var day in result.Days.Zip(result.Timetable))
            {
                foreach (var session in BuildDay(day.First, day.Second))
                {
                    session.Group = group;
                    group.Disciplines.Add<Discipline>(session.Discipline);
                }
            }
        }

        private static IEnumerable<TrainingSession> BuildDay(DateTime date, string[] day_sessions)
        {
            for (int i = 0; i < day_sessions.Length; i += 2)
            {
                if (day_sessions[i] == "") continue;
                
                foreach (var session in BuildCell(day_sessions[i], day_sessions[i + 1], date, i / 2 + 1))
                {
                    yield return session;
                }
            }
        }

        private static IEnumerable<TrainingSession> BuildCell(string cell, string room, DateTime date, int number)
        {
            var distRows = cell.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split('\n');
            var roomRows = room.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split('\n');

            for (int row = 0; row < distRows.Length; row += 3)
            {
                var discipline = AddDiscipline(distRows[row]);
                var teacher = AddTeacher(distRows[row + 2]);

                discipline.Teachers.Add<Teacher>(teacher);
                teacher.Disciplines.Add<Discipline>(discipline);

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

        private static Discipline AddDiscipline(string discipline_name)
        {
            var discipline = objectTree.Disciplines.FirstOrDefault(d => d.Name == discipline_name);

            if (discipline is null)
            {
                discipline = new Discipline();
                discipline.Name = discipline_name;
                objectTree.Disciplines.Add(discipline);
            }

            return discipline;
        }
        private static Teacher AddTeacher(string teacher_name)
        {
            var teacher = objectTree.Teachers.FirstOrDefault(d => d.Name == teacher_name);

            if (teacher is null)
            {
                teacher = new Teacher();
                teacher.Name = teacher_name;
                objectTree.Teachers.Add(teacher);
            }

            return teacher;
        }
        private static void Add<T>(this ICollection<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}