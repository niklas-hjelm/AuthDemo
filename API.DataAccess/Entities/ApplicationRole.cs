using Microsoft.AspNetCore.Identity;

namespace API.DataAccess.Entities;

public class ApplicationRole: IdentityRole
{
    public ApplicationRole() { }

    public ApplicationRole(string roleName) : base(roleName) { }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
}
