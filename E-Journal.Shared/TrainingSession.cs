using System;
using System.Collections.Generic;

namespace E_Journal.Shared
{
    public class TrainingSession
    {
        public TrainingSession()
        {
            StudentStatuses = new List<StudentStatus>();
            Room = String.Empty;
            Subgroup = ' ';
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public byte Number { get; set; }
        public string Room { get; set; }
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public char Subgroup { get; set; }
        public ICollection<StudentStatus> StudentStatuses { get; set; }

        public override string ToString()
        {
            return Discipline?.Name ?? "<дисциплина не определена>";
        }
    }
}
