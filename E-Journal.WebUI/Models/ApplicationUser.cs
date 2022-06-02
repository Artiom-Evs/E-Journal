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

        [PersonalData]
        public string FirstName { get; set; } = string.Empty;
        [PersonalData]
        public string SecondName { get; set; } = string.Empty;
        [PersonalData]
        public string LastName { get; set; } = string.Empty;

        public int AssociatedId { get; set; }
        public bool UserConfirmed { get; set; }
    }
}
