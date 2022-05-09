using E_Journal.Shared;
using E_Journal.Parser;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.Infrastructure
{
    public class TimetableBuilder
    {
        private readonly JournalDbContext _context;

        public TimetableBuilder(JournalDbContext context)
        {
            this._context = context;
        }

        public IEnumerable<Lesson> BuildWeekLessons(ParseResult result)
        {
            Group group = GetOrCreateGroup(result.Name);
            
            if (result.Days is null || result.LessonsText is null)
            {
                yield break;
            }

            foreach (var (date, dayLessonsText) in result.Days.Zip(result.LessonsText))
            {
                foreach (var lesson in BuildDayLessons(date, dayLessonsText))
                {
                    lesson.Group = group;
                    yield return lesson;
                }
            }
        }
        private IEnumerable<Lesson> BuildDayLessons(DateTime date, string[] dayLessonsText)
        {
            for (int i = 0; i < dayLessonsText.Length; i += 2)
            {
                if (dayLessonsText[i] == "") continue;

                foreach (var lesson in BuildCellLessons(dayLessonsText[i], dayLessonsText[i + 1], date, i / 2 + 1))
                {
                    yield return lesson;
                }
            }
        }
        private IEnumerable<Lesson> BuildCellLessons(string lessonCell, string roomCell, DateTime date, int lessonNumber)
        {
            var lessonRows = lessonCell.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split("\r\n");
            var roomRows = roomCell.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split("\r\n");

            for (int row = 0; row < lessonRows.Length / 3; row++)
            {
                var discipline = GetOrCreateDiscipline(lessonRows[(row * 3)]);
                var teacher = GetOrCreateTeacher(lessonRows[(row * 3) + 2]);

                Lesson lesson = new()
                {
                    Discipline = discipline,
                    Teacher = teacher,
                    Date = date, 
                    // вычисляется индекс для получения номера кабинета
                    Room = (row < roomRows.Length) ? roomRows[row] : roomRows[0], 
                    Number = lessonNumber
                };
                    
                if (lessonRows.Length > 3)
                {
                    lesson.Subgroup = char.Parse((row + 1).ToString());
                }

                yield return lesson;
            }
        }

        private Group GetOrCreateGroup(string group_name)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Name == group_name);

            if (group == null)
            {
                group = new Group(group_name);
                _context.Groups.Add(group);
                _context.SaveChanges();
            }

            return group;
        }
        private Discipline GetOrCreateDiscipline(string discipline_name)
        {
            var discipline = _context.Disciplines.FirstOrDefault(d => d.Name == discipline_name);

            if (discipline == null)
            {
                discipline = new Discipline(discipline_name);
                _context.Disciplines.Add(discipline);
                _context.SaveChanges();
            }

            return discipline;
        }
        private Teacher GetOrCreateTeacher(string teacher_name)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.Name == teacher_name);

            if (teacher == null)
            {
                teacher = new Teacher(teacher_name);
                _context.Teachers.Add(teacher);
                _context.SaveChanges();
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
