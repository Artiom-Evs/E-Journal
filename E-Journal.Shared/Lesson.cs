using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Journal.Shared
{
    public class Lesson
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int DisciplineId { get; set; }
        public int TeacherId { get; set; }
        public Schedule Schedule { get; set; }
        public Discipline Discipline { get; set; }
        public Teacher Teacher { get; set; }
        public int Number { get; set; }
        public char Subgroup { get; set; }
        public string Room { get; set; }

        public Lesson() { }
        public Lesson(Schedule schedule, Discipline discipline, Teacher teacher, string room)
        {
            Schedule = schedule;
            Discipline = discipline;
            Teacher = teacher;
            Room = room;
        }
    }
}
