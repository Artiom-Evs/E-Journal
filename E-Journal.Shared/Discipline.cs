namespace E_Journal.Shared;

public class Discipline
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Discipline() { }
    public Discipline(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
