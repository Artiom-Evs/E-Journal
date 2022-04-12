namespace E_Journal.WebUI.Models
{
    public record GroupScheduleDataModel
    {
        public int GroupId { get; init; }
        public int[] ActualScheduleIds { get; init; } = Array.Empty<int>();
    }
}
