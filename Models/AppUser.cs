using Microsoft.AspNetCore.Identity;

namespace WebApplicationMVC.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
