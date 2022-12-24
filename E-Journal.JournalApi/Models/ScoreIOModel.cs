using System.ComponentModel.DataAnnotations;

namespace E_Journal.JournalApi.Models;

public record ScoreIOModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StudentId { get; init; }

    [Required]
    public DateTime Date { get; init; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Number { get; init; }
    
    [Required]
    public string Subject { get; init; }
    [Required]
    public string Type { get; init; }
    [Required]
    public string Teacher { get; init; }
    [Required]
    public string Value { get; init; }
}
