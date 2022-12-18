using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Models;

public class Student
{
    public Student()
    {
    }

    public Student(string name, Group group)
    {
        Name = name;
        Group = group;
    }

    public int Id { get; init; }
    public string Name { get; set; }
    public Group Group { get; set; }
    public int GroupId { get; set; }

    public override string ToString()
    {
        return
            $"{nameof(this.Name)}: {this.Name}\r\n" +
            $"{nameof(this.Group)}: {this.Group?.Name ?? "NULL"}\r\n";
    }
}
