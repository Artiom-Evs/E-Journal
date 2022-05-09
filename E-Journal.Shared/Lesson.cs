namespace E_Journal.Shared;

public class Lesson
{
    public int Id { get; set; }
    public int DisciplineId { get; set; }
    public int TeacherId { get; set; }
    public int GroupId { get; set; }

    public Discipline Discipline { get; set; }
    public Teacher Teacher { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Group Group { get; set; }
    public char Subgroup { get; set; }
        
    public DateTime Date { get; set; }
    public string Room { get; set; } = string.Empty;
    public int Number { get; set; }


    public Lesson() { }
        
    public override bool Equals(object? obj)
    {
        if (obj is Lesson lesson)
        {
            return this.DisciplineId == lesson.DisciplineId
                && this.TeacherId == lesson.TeacherId
                && this.GroupId == lesson.GroupId
                && this.Subgroup == lesson.Subgroup
                && this.Date == lesson.Date
                && this.Room == lesson.Room
                && this.Number == lesson.Number;
        }

        return false;
    }

    public override int GetHashCode()
    {
        long hCode = this.DisciplineId ^
                this.TeacherId ^
                this.GroupId ^
                this.Subgroup ^
                this.Date.Ticks ^
                this.Room.GetHashCode() ^
                this.Number;

        return hCode.GetHashCode();
    }
}
