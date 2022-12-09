namespace E_Journal.SchedulesApi.Models;

public abstract record BaseModel : IBaseModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public override string ToString()
    {
        return this.Name;
    }
}
