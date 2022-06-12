#nullable disable

namespace E_Journal.WebUI.Models.ViewModels;

public record StudentViewModel
{
    public int Id { get; set; }

    public string FirstName { get; init; }
    public string SecondName { get; init; }
    public string LastName { get; init; }
    public string Group { get; init; }

    public string GetInitials() =>
            $"{SecondName} {FirstName[0]}. {LastName[0]}.";
}
