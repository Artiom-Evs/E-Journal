using System;
using System.Collections.Generic;

namespace E_Journal.Shared
{
    public class TrainingSession
    {
        public TrainingSession()
        {
            StudentStatuses = new List<StudentStatus>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Discipline Discipline { get; set; }
        public Teacher Teacher { get; set; }
        public Group Group { get; set; }
        public ICollection<StudentStatus> StudentStatuses { get; set; }
    }
}
