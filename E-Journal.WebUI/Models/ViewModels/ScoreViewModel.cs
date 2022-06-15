#nullable disable

using E_Journal.Shared;

namespace E_Journal.WebUI.Models.ViewModels;

public record ScoreViewModel
{
    public int StudentId { get; init; }
    public string StudentInitials { get; set; } 
    public string Value { get; init; }
    public bool IsMarked { get; set; }
}
