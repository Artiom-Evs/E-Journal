using System;
using System.Collections.Generic;

namespace E_Journal.Shared
{
    public class Discipline
    {
        public Discipline()
        {
            Name = "";
            Groups = new List<Group>();
            Teachers = new List<Teacher>();
            TrainingSessions = new List<TrainingSession>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
        public ICollection<TrainingSession> TrainingSessions { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
