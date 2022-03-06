﻿using System;
using System.Collections.Generic;

namespace E_Journal.Shared
{
    public class Teacher
    {
        public Teacher(string name)
        {
            Name = name;
            Disciplines = new List<Discipline>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Discipline> Disciplines { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
