#nullable disable

namespace E_Journal.Shared;

public record ScoreValue
{
    public int Id { get; init; }
    public string Title { get; init; }

    public override string ToString()
    {
        return this.Title.ToString();
    }
}
