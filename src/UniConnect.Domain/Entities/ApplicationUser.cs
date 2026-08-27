using Microsoft.AspNetCore.Identity;
using UniConnect.Domain.Enums;

namespace UniConnect.Domain.Entities;

// Inherit from IdentityUser<Guid>
public class ApplicationUser : IdentityUser<Guid>
{
    public UserType UserType { get; set; }
    public bool IsActive { get; set; } = true;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public UserProfile? Profile { get; set; }
    public BusinessProfile? BusinessProfile { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}