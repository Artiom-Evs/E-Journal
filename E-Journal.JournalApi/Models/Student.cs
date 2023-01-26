using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi.Models;

public record Student : BaseModel
{
    public Group? Group { get; set; }
    public int GroupId { get; set; }
}
