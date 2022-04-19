using Microsoft.AspNetCore.Identity;

namespace E_Journal.WebUI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }

        public int AssociatedId { get; set; }
        public bool UserConfirmed { get; set; }
    }
}
