#nullable disable

using E_Journal.Parser.Models;
using System.Text.Json.Serialization;

namespace E_Journal.Parser.Models;

public class Lesson
{
    [JsonIgnore]
    public int Id { get; init; }
    public string Title { get; init; }
    public string Type { get; init; }
    public string Subtitle { get; init; }
    public string Room { get; init; }
    public int Number { get; init; }
    public int? Subgroup { get; init; }

    public override bool Equals(object obj)
    {
        if (obj is Lesson lesson)
        {
            return Title.Equals(lesson.Title) &&
                Type.Equals(lesson.Type) &&
                Subtitle.Equals(lesson.Subtitle) &&
                Room.Equals(lesson.Room) &&
                Number.Equals(lesson.Number) &&
                Subgroup.Equals(lesson.Subgroup);
        }

        return false;
    }

    public override int GetHashCode()
    {
        string str = Title +
            Type +
            Subtitle +
            Room +
            Number.ToString() +
            Subgroup ?? "NULL";

        return str.GetHashCode();
    }
}
