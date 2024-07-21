using Microsoft.AspNetCore.Identity;

namespace ReviewsDashboard.Privilage
{
    public class ExtendIdentityUser: IdentityUser
    {
        public string FullName { get; set; }
    }
}
