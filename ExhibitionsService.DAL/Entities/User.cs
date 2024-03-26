using Microsoft.AspNetCore.Identity;

namespace ExhibitionsService.DAL.Entities
{
    public class User : IdentityUser<int>
    {
        public UserProfile UserProfile { get; set; }
    }
}
