using Microsoft.AspNetCore.Identity;

namespace E_Journal.Identity.Models;

public class ApplicationUser : IdentityUser
{
    public string Initials { get; set; }
}