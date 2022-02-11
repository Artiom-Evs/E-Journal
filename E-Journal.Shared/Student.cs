namespace E_Journal.Shared
{
    public class Student
    {
        public Student()
        {
            Name = "";
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Group Group { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
