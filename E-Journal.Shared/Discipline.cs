using System;
using System.Collections.Generic;

namespace E_Journal.Shared
{
    public class Discipline
    {
        public Discipline(string name)
        {
            Name = name;
            Groups = new List<Group>();
            Teachers = new List<Teacher>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
