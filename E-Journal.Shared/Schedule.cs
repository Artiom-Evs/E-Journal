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

        public override bool Equals(object? obj)
        {
            if (obj is Schedule schedule)
            {
                return Date == Date
                    && GroupId == GroupId
                    && new HashSet<Lesson>(this.Lessons).SetEquals(schedule.Lessons);
            }

            return false;
        }

        public override int GetHashCode()
        {
            string str = $"{this.Date}{this.GroupId}";

            this.Lessons
                .Select(l => l.GetHashCode())
                .OrderBy(num => num)
                .ToList()
                .ForEach(hash => str += hash.ToString());

            return str.GetHashCode();
        }
    }
}
