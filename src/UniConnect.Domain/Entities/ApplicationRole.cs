using Microsoft.AspNetCore.Identity;

namespace UniConnect.Domain.Entities;

// Inherit from IdentityRole<Guid>
public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName) { }

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}