namespace E_Journal.Shared;

public class Student
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Group Group { get; set; }

    public Student() { }
    public Student(string firstName, string secondName, string lastName, Group group)
    {
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        Group = group;
    }

    public string GetInitials() =>
        $"{SecondName} {FirstName[0]}. {LastName[0]}.";

    public override string ToString()
    {
        return GetInitials();
    }
}
