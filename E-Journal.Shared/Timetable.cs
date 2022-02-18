using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Journal.Shared
{
    public class Timetable
    {
        public Timetable()
        {
            TrainingSessions = new List<TrainingSession>();
        }

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public ICollection<TrainingSession> TrainingSessions { get; set; }
    }
}
