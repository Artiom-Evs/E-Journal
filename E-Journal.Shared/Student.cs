namespace E_Journal.Shared;

public class Student
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public Group Group { get; set; }

    public Student() { }
    public Student(string name, Group group)
    {
        Name = name;
        Group = group;
    }

    public override string ToString()
    {
        return Name;
    }
}
