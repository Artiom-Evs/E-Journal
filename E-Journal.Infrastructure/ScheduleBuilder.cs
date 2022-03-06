using E_Journal.Shared;
using E_Journal.Parser;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.Infrastructure
{
    public class TimetableBuilder
    {
        private readonly JournalDbContext dbContext;

        public TimetableBuilder(JournalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Group BuildWeekSchedules(ParseResult result)
        {
            Group group = GetOrCreateGroup(result.Name);

            if (result.Days is null || result.TextSchedules is null)
            {
                return group;
            }

            foreach (var (date, textSchedule) in result.Days.Zip(result.TextSchedules))
            {
                group.Schedules.Add(BuildSchedule(date, textSchedule, group));
            }

            return group;
        }
        private Schedule BuildSchedule(DateTime date, string[] textSchedule, Group group)
        {
            Schedule schedule = new(group, date);

            for (int i = 0; i < textSchedule.Length; i += 2)
            {
                if (textSchedule[i] == "") continue;

                foreach (var lesson in BuildLessons(textSchedule[i], textSchedule[i + 1], date, i / 2 + 1, schedule))
                {
                    schedule.Lessons.Add(lesson);
                }
            }

            return schedule;
        }
        private IEnumerable<Lesson> BuildLessons(string lessonCell, string roomCell, DateTime date, int lessonNumber, Schedule schedule)
        {
            var lessonRows = lessonCell.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split("\r\n");
            var roomRows = roomCell.Replace("1.", "").Replace("2.", "").Replace("3.", "").Split("\r\n");

            for (int row = 0; row < lessonRows.Length / 3; row++)
            {
                var discipline = GetOrCreateDiscipline(lessonRows[(row * 3)]);
                var teacher = GetOrCreateTeacher(lessonRows[(row * 3) + 2]);

                Add<Teacher>(discipline.Teachers, teacher);
                Add<Discipline>(teacher.Disciplines, discipline);
                Add<Discipline>(schedule.Group.Disciplines, discipline);
                Add<Group>(discipline.Groups, schedule.Group);

                // последним аргументом в конструкторе вычисляется индекс для получения номера кабинета
                Lesson lesson = new(schedule, discipline, teacher, (row < roomRows.Length) ? roomRows[row] : roomRows[0]);
                lesson.Number = lessonNumber;

                if (lessonRows.Length > 3)
                {
                    lesson.Subgroup = char.Parse((row + 1).ToString());
                }

                yield return lesson;
            }
        }

        private Group GetOrCreateGroup(string group_name)
        {
            var group = dbContext.Groups.FirstOrDefault(g => g.Name == group_name);

            if (group == null)
            {
                group = new Group(group_name);
                dbContext.Groups.Add(group);
                dbContext.SaveChanges();
            }

            return group;
        }
        private Discipline GetOrCreateDiscipline(string discipline_name)
        {
            var discipline = dbContext.Disciplines.FirstOrDefault(d => d.Name == discipline_name);

            if (discipline == null)
            {
                discipline = new Discipline(discipline_name);
                dbContext.Disciplines.Add(discipline);
                dbContext.SaveChanges();
            }

            return discipline;
        }
        private Teacher GetOrCreateTeacher(string teacher_name)
        {
            var teacher = dbContext.Teachers.FirstOrDefault(t => t.Name == teacher_name);

            if (teacher == null)
            {
                teacher = new Teacher(teacher_name);
                dbContext.Teachers.Add(teacher);
                dbContext.SaveChanges();
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
