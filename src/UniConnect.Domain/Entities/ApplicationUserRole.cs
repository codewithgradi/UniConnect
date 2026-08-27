using Microsoft.AspNetCore.Identity;

namespace UniConnect.Domain.Entities;

// Inherit from IdentityUserRole<Guid>
public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationRole Role { get; set; } = null!;
}