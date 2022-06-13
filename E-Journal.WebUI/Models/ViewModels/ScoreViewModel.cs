#nullable disable

using E_Journal.Shared;

namespace E_Journal.WebUI.Models.ViewModels;

public record ScoreViewModel
{
    public int ScoreId { get; init; }
    public string StudentInitials { get; set; }
    public ScoreValue Value { get; init; }
}
