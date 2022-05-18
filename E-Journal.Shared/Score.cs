namespace E_Journal.Shared;

public class Score
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int LessonId { get; set; }

    public Student Student { get; set; }
    public Lesson Lesson { get; set; }

    public ScoreValue Value { get; set; }

    public Score() { }
    public Score(Student student, Lesson lesson, ScoreValue value)
    {
        Student = student;
        Lesson = lesson;
        Value = value;
    }

    public override string ToString()
    {
        return this.Value.ToString();
    }
}
