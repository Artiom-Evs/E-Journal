using System;
using System.Text.Json.Serialization;

namespace E_Journal.Parser.Models;

public class DaylySchedule
{
    [JsonIgnore]
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime Date { get; init; } = default;
    public ICollection<Lesson>? Lessons { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is DaylySchedule schedule)
        {
            return Title.Equals(schedule.Title) &&
                Date.Equals(schedule.Date) &&
                (Lessons ?? Array.Empty<Lesson>()).SequenceEqual(schedule.Lessons ?? Array.Empty<Lesson>());
        }

        return false;
    }

    public override int GetHashCode()
    {
        string str = Date.ToString() +
            string.Join("", Lessons?.Select(l => l.GetHashCode().ToString()) ?? Array.Empty<string>());

        return str.GetHashCode();
    }
}
