namespace E_Journal.SchedulesApi.Models;

public class OutputLessonModel
{
    public OutputLessonModel(Lesson lesson)
    {
        this.Group = lesson.Group.Name;
        this.Subject = lesson.Subject.Name;
        this.Type = lesson.Type.Name;
        this.Teacher = lesson.Teacher.Name;
        this.Room = lesson.Room.Name;
        this.Date = lesson.Date;
        this.Number = lesson.Number;
        this.Subgroup = lesson.Subgroup;
    }

    public string Group { get; set; }
    public string Subject { get; set; }
    public string Type { get; set; }
    public string Teacher { get; set; }
    public string Room { get; set; }
    public DateTime Date { get; set; }
    public int Number { get; set; }
    public int Subgroup { get; set; }
}
