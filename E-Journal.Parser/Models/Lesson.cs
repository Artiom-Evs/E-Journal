﻿#nullable disable

using E_Journal.Parser.Models;
using System.Text.Json.Serialization;

namespace E_Journal.Parser.Models;

public class Lesson
{
    public int Id { get; init; }
    public DateTime Date { get; set; }
    public string Title { get; init; }
    public string Type { get; init; }
    public string TeatherName { get; init; }
    public string GroupName { get; init; }
    public string Room { get; init; }
    public int Number { get; init; }
    public int? Subgroup { get; init; }

    public override bool Equals(object obj)
    {
        if (obj is Lesson lesson)
        {
            return GroupName.Equals(lesson.GroupName) &&
                Title.Equals(lesson.Title) &&
                Type.Equals(lesson.Type) &&
                TeatherName.Equals(lesson.TeatherName) &&
                Room.Equals(lesson.Room) &&
                Number.Equals(lesson.Number) &&
                Subgroup.Equals(lesson.Subgroup);
        }

        return false;
    }

    public override int GetHashCode()
    {
        string str = GroupName +
            Title +
            Type +
            TeatherName +
            Room +
            Number.ToString() +
            Subgroup ?? "NULL";

        return str.GetHashCode();
    }
}