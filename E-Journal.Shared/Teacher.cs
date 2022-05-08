namespace E_Journal.Shared;

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Teacher() { }
    public Teacher(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
