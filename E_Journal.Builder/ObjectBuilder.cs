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
            objectTree = new();
            objectTree.Sessions = new();
            objectTree.Disciplines = new();
            objectTree.Teachers = new();
            objectTree.Groups = new();

            foreach (var result in results)
            {
                BuildGroup(result);
            }

            var objects = objectTree;
            objectTree = new();

            return (objects.Sessions?.ToArray(), objects.Disciplines?.ToArray(), objects.Teachers?.ToArray(), objects.Groups?.ToArray());
        }

        public static Group BuildGroup(ParseResult result)
        {
            Group group = new() { Name = result.Name };
            objectTree.Groups.Add(group);

            string cell;
            string room;
            DateTime date;

            if (result.Days is null || result.Timetable is null)
            {
                return group;
            }

            for (int row = 0; row < result.Timetable?.Length; row++)
            {
                for (int col = 0; col < result.Timetable?[row].Length;)
                {
                    if (result.Timetable[row][col] == "") break;

                    date = result.Days[col / 2];
                    cell = result.Timetable[row][col++];
                    room = result.Timetable[row][col++];


                    var sessions = ParseCell(cell, room, date);

                    foreach (var session in sessions)
                    {
                        session.Group = group;
                        group.Disciplines.Add(session.Discipline);
                    }
                }
            }

            group.Disciplines = group.Disciplines.Distinct().ToList();
            return group;
        }

        private static TrainingSession[] ParseCell(string cell, string room, DateTime date)
        {
            List<TrainingSession> sessions = new();
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
                    Room = (row / 3 < roomRows.Length) ? roomRows[row / 3] : roomRows[0]
                };

                sessions.Add(session);
            }

            return sessions.ToArray();
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