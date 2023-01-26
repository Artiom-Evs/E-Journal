namespace E_Journal.JournalApi.Models;

public record Training : IBaseModel
{
    public int Id { get; set; }
    public string Description { get; set; }

    public Group? Group { get; set; }
    public int GroupId { get; set; }
    public DateTime Date { get; set; }
    public int Number { get; set; }

    public Subject? Subject { get; set; }
    public int SubjectId { get; set; }
    public TrainingType? Type { get; set; }
    public int TypeId { get; set; }
    public Teacher? Teacher { get; set; }
    public int TeacherId { get; set; }
}
