using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Journal.Shared
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public ICollection<Lesson> Lessons { get; set; }

        public Schedule() { }
        public Schedule(Group group, DateTime date)
        {
            Group = group;
            Date = date;
            Lessons = new List<Lesson>();
        }
    }
}
