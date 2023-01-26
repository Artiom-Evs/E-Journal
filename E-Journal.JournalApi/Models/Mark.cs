namespace E_Journal.JournalApi.Models;

public class Mark : IBaseModel
{
    public int Id { get; set; }
    public Student? Student { get; set; }
    public int StudentId { get; set; }
    public Training? Training { get; set; }
    public int TrainingId { get; set; }
    public MarkValue? Value { get; set; }
    public int ValueId { get; set; }
}
