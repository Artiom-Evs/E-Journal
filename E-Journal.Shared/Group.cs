using System;
using System.Collections.Generic;

namespace E_Journal.Shared
{
    public class Group
    {
        public Group(string name)
        {
            Name = name;
            Students = new List<Student>();
            Disciplines = new List<Discipline>();
            Schedules = new List<Schedule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Discipline> Disciplines { get; set; }
        public ICollection<Schedule> Schedules { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
