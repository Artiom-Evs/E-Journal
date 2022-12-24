using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace E_Journal.JournalApi.Models;

public class StudentIOModel
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Group { get; set; } = string.Empty;
    [Required]
    public int GroupId { get; set; }
}
