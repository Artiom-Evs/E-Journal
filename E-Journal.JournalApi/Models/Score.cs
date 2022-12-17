﻿namespace E_Journal.JournalApi.Models;

public class Score
{
    public Score()
    {
    }

    public Score(Student student, Subject subject, Type type, Teacher teacher, DateTime date, int number, int subgroup)
    {
        Student = student;
        Subject = subject;
        Type = type;
        Teacher = teacher;
        Date = date;
        Number = number;
        Subgroup = subgroup;
    }

    public Student Student { get; set; }
    public int StudentId { get; set; }

    public Subject Subject { get; set; }
    public int SubjectId { get; set; }
    public Type Type { get; set; }
    public int TypeId { get; set; }
    public Teacher Teacher { get; set; }
    public int TeacherId { get; set; }

    public DateTime Date { get; set; }
    public int Number { get; set; }
    public int Subgroup { get; set; }
    
    public ScoreValue Value { get; set; }

    public override string ToString()
    {
        return
            $"{nameof(this.Student)}: {this.Student?.Name ?? "NULL"}\r\n" +
            $"{nameof(this.Subject)}: {this.Subject?.Name ?? "NULL"}\r\n" +
            $"{nameof(this.Type)}: {this.Type?.Name ?? "NULL"}\r\n" +
            $"{nameof(this.Teacher)}: {this.Teacher?.Name ?? "NULL"}\r\n" +
            $"{nameof(this.Date)}: {this.Date.ToString("d")}\r\n" +
            $"{nameof(this.Number)}: {this.Number}\r\n" +
            $"{nameof(this.Subgroup)}: {this.Subgroup}\r\n" +
            $"{nameof(this.Value)}: {this.Value?.Name ?? "NULL"}\r\n";
    }
}
