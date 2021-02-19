using Microsoft.AspNetCore.Identity;

namespace IdentityStandaloneUserCheck.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string LastLogin { get; set; }
    }
}