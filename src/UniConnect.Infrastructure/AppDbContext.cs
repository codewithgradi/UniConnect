using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain;
using UniConnect.Domain.Entities;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    IdentityUserClaim<Guid>,
    ApplicationUserRole,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // User & Profiles
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();

    // Skills & Endorsements
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<SkillEndorsement> SkillEndorsements => Set<SkillEndorsement>();

    // Career & Academic Details
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Certification> Certifications => Set<Certification>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();

    // Social Features
    public DbSet<Connection> Connections => Set<Connection>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<DirectMessage> DirectMessages => Set<DirectMessage>();

    // Opportunities, Events & System Features
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<InstitutionalEvent> InstitutionalEvents => Set<InstitutionalEvent>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. MUST execute base Identity configuration first
        base.OnModelCreating(modelBuilder);

        // 2. Identity Join Configuration Overrides
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.HasMany(e => e.UserRoles)
             .WithOne(e => e.User)
             .HasForeignKey(ur => ur.UserId)
             .IsRequired();
        });

        modelBuilder.Entity<ApplicationRole>(b =>
        {
            b.HasMany(e => e.UserRoles)
             .WithOne(e => e.Role)
             .HasForeignKey(ur => ur.RoleId)
             .IsRequired();
        });

        // 3. One-to-One Profiles
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.BusinessProfile)
            .WithOne(b => b.User)
            .HasForeignKey<BusinessProfile>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Many-to-Many: User Skills
        modelBuilder.Entity<UserSkill>()
            .HasKey(us => new { us.UserProfileId, us.SkillId });

        modelBuilder.Entity<UserSkill>()
            .HasOne(us => us.UserProfile)
            .WithMany(p => p.UserSkills)
            .HasForeignKey(us => us.UserProfileId);

        modelBuilder.Entity<UserSkill>()
            .HasOne(us => us.Skill)
            .WithMany(s => s.UserSkills)
            .HasForeignKey(us => us.SkillId);

        // 5. Skill Endorsements
        modelBuilder.Entity<SkillEndorsement>()
            .HasKey(se => se.Id);

        modelBuilder.Entity<SkillEndorsement>()
            .HasOne(se => se.UserSkill)
            .WithMany(us => us.Endorsements)
            .HasForeignKey(se => new { se.UserSkillUserProfileId, se.UserSkillSkillId })
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SkillEndorsement>()
            .HasOne(se => se.EndorsedBy)
            .WithMany()
            .HasForeignKey(se => se.EndorsedByUserId)
            .OnDelete(DeleteBehavior.Restrict);



        // 6. Recommendations
        modelBuilder.Entity<Recommendation>(b =>
        {
            b.HasKey(r => r.Id);

            b.HasOne(r => r.AuthorProfile)
             .WithMany(p => p.GivenRecommendations)
             .HasForeignKey(r => r.AuthorProfileId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(r => r.TargetProfile)
             .WithMany(p => p.ReceivedRecommendations)
             .HasForeignKey(r => r.TargetProfileId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // 7. Social Connections
        modelBuilder.Entity<Connection>(b =>
        {
            b.HasKey(c => c.Id);

            b.HasOne(c => c.Requester)
             .WithMany()
             .HasForeignKey(c => c.RequesterId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(c => c.Receiver)
             .WithMany()
             .HasForeignKey(c => c.ReceiverId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // 8. Posts, Comments & Reactions
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.Post)
            .WithMany(p => p.Reactions)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reactions)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 9. Direct Messages
        modelBuilder.Entity<DirectMessage>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DirectMessage>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // 10. Opportunities & Job Applications
        modelBuilder.Entity<Opportunity>()
            .HasOne(o => o.BusinessProfile)
            .WithMany(b => b.Opportunities)
            .HasForeignKey(o => o.BusinessProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplication>()
            .HasOne(a => a.Opportunity)
            .WithMany(o => o.Applications)
            .HasForeignKey(a => a.OpportunityId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplication>()
            .HasOne(a => a.Applicant)
            .WithMany()
            .HasForeignKey(a => a.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        // 11. Institutional Events & Notifications
        modelBuilder.Entity<InstitutionalEvent>()
            .HasOne(e => e.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(e => e.CreatedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}