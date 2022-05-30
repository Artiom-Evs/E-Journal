#nullable disable

namespace E_Journal.WebUI.Models.ViewModels;

public record ApplicationUserViewModel
{
    public string Id { get; init; }
    public string Email { get; init; } 
    public string Role { get; init; }
    public bool IsConfirmed { get; init; }
}
