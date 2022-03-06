namespace E_Journal.Shared
{
    public class Student
    {
        public Student() { }
        public Student(string name, Group group)
        {
            Name = name;
            Group = group;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
