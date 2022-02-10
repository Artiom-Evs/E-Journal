﻿using System;
using System.Collections.Generic;

namespace E_Journal.Shared
{
    public class Group
    {
        public Group()
        {
            Name = "";
            Students = new List<Student>();
            Disciplines = new List<Discipline>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Discipline> Disciplines { get; set; }
    }
}