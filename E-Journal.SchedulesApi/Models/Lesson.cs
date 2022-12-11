namespace E_Journal.SchedulesApi.Models;

public class Lesson
{
    public Lesson()
    {
    }

    public Lesson(Subject subject, Type type, Teacher teacher, Group group, Room room, int number, int subgroup, DateTime date)
    {
        Subject = subject;
        Type = type;
        Teacher = teacher;
        Group = group;
        Room = room;
        Number = number;
        Subgroup = subgroup;
        Date = date;
    }

    public Subject Subject { get; set; }
    public int SubjectId { get; set; }
    public Type Type { get; set; }
    public int TypeId { get; set; }
    public Teacher Teacher { get; set; }
    public int TeacherId { get; set; }
    public Group Group { get; set; }
    public int GroupId { get; set; }
    public Room Room { get; set; }
    public int RoomId { get; set; }

    public int Number { get; set; }
    public int Subgroup { get; set; }
    public DateTime Date { get; set; }

    public override string ToString()
    {
        return $"{nameof(Subject)}: {this.Subject?.Name ?? "NULL"}\r\n" +
            $"{nameof(Type)}: {this.Type?.Name ?? "NULL"}\r\n" +
            $"{nameof(Teacher)}: {this.Teacher?.Name ?? "NULL"}\r\n" +
            $"{nameof(Group)}: {this.Group?.Name ?? "NULL"}\r\n" +
            $"{nameof(Room)}: {this.Room?.Name ?? "NULL"}\r\n" +
            $"{nameof(Subgroup)}: {this.Subgroup}\r\n" +
            $"{nameof(Number)}: {this.Number}\r\n" +
            $"{nameof(Date)}: {this.Date.ToString("d")}";
    }
}
