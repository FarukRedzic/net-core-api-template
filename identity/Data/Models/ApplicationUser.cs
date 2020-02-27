using Microsoft.AspNetCore.Identity;

namespace identity.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
    }
}
