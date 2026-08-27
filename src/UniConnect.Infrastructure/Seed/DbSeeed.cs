using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;

namespace Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // Ensure database is available before seeding
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Roles (4)
        string[] roleNames = { "Student", "Alumni", "Business", "Admin" };
        var roles = new List<ApplicationRole>();
        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName);
                await roleManager.CreateAsync(role);
            }
            roles.Add(role);
        }

        // Prevent duplicate execution if users already exist
        if (await context.Users.AnyAsync()) return;

        // 2. Seed Users (30)
        var users = new List<ApplicationUser>();
        for (int i = 1; i <= 30; i++)
        {
            var userType = (i <= 18) ? UserType.Student : (i <= 25 ? UserType.Alumni : (i <= 28 ? UserType.Business : UserType.Admin));
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = $"user{i}@uniconnect.ac.za",
                Email = $"user{i}@uniconnect.ac.za",
                EmailConfirmed = true,
                UserType = userType,
                IsActive = true,
                VerificationStatus = UniConnect.Domain.Enums.VerificationStatus.Approved,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-i)
            };

            await userManager.CreateAsync(user, "Password123!");

            // Assign Roles
            string assignedRole = userType switch
            {
                UserType.Student => "Student",
                UserType.Alumni => "Alumni",
                UserType.Business => "Business",
                _ => "Admin"
            };
            await userManager.AddToRoleAsync(user, assignedRole);
            users.Add(user);
        }

        // 3. Seed Skills (30)
        var skillNames = new[]
        {
            "C#", ".NET Core", "PostgreSQL", "React", "TypeScript", "Python", "SQL", "Docker", "AWS", "Git",
            "REST APIs", "Clean Architecture", "Entity Framework", "JavaScript", "HTML5", "CSS3", "Tailwind",
            "Node.js", "GraphQL", "Redis", "CI/CD", "Unit Testing", "Microservices", "System Design", "Agile",
            "Scrum", "Kubernetes", "Azure", "Figma", "Data Structures"
        };
        var skills = skillNames.Select(name => new Skill { Id = Guid.NewGuid(), Name = name }).ToList();
        await context.Skills.AddRangeAsync(skills);
        await context.SaveChangesAsync();

        // 4. Seed UserProfiles (30)
        var userProfiles = new List<UserProfile>();
        for (int i = 0; i < 30; i++)
        {
            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = users[i].Id,
                FirstName = $"FirstName{i + 1}",
                LastName = $"LastName{i + 1}",
                SystemHeadline = $"Software Engineer Candidate #{i + 1}",
                AboutBio = $"Enthusiastic developer and student at UniConnect pursuing technology solutions for profile {i + 1}.",
                GithubUrl = $"https://github.com/user{i + 1}",
                CvFileUrl = $"https://storage.uniconnect.ac.za/cvs/user_{i + 1}.pdf",
                IsPublic = true,
                Programme = (i % 2 == 0) ? "BSc Computer Science" : "Diploma in Information Technology"
            };
            userProfiles.Add(profile);
        }
        await context.UserProfiles.AddRangeAsync(userProfiles);
        await context.SaveChangesAsync();

        // 5. Seed BusinessProfiles (30)
        var businessProfiles = new List<BusinessProfile>();
        for (int i = 0; i < 30; i++)
        {
            var bp = new BusinessProfile
            {
                Id = Guid.NewGuid(),
                UserId = users[i % users.Count].Id,
                CompanyName = $"Tech Corp {i + 1}",
                CompanyRegistrationNumber = $"REG-2026-{1000 + i}",
                WebsiteUrl = $"https://techcorp{i + 1}.com",
                Industry = (i % 3 == 0) ? "FinTech" : (i % 3 == 1 ? "EdTech" : "Software Solutions"),
                Description = $"Leading provider of software services and innovation #{i + 1}."
            };
            businessProfiles.Add(bp);
        }
        await context.BusinessProfiles.AddRangeAsync(businessProfiles);
        await context.SaveChangesAsync();

        // 6. Seed Posts (30)
        var posts = new List<Post>();
        for (int i = 0; i < 30; i++)
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                AuthorId = users[i % users.Count].Id,
                Content = $"Excited to share update #{i + 1} on my latest Clean Architecture project built with ASP.NET Core & PostgreSQL!",
                CreatedAtUtc = DateTime.UtcNow.AddHours(-i)
            };
            posts.Add(post);
        }
        await context.Posts.AddRangeAsync(posts);
        await context.SaveChangesAsync();

        // 7. Seed Comments (30)
        var comments = new List<Comment>();
        for (int i = 0; i < 30; i++)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = posts[i % posts.Count].Id,
                AuthorId = users[(i + 1) % users.Count].Id,
                Content = $"Great insights on post #{i + 1}! Keep up the solid work.",
                CreatedAtUtc = DateTime.UtcNow.AddMinutes(-i * 10)
            };
            comments.Add(comment);
        }
        await context.Comments.AddRangeAsync(comments);

        // 8. Seed Reactions (30)
        var reactions = new List<Reaction>();
        for (int i = 0; i < 30; i++)
        {
            var reaction = new Reaction
            {
                Id = Guid.NewGuid(),
                PostId = posts[i % posts.Count].Id,
                UserId = users[(i + 2) % users.Count].Id,
                ReactionType = (i % 2 == 0) ? "Like" : "Celebrate"
            };
            reactions.Add(reaction);
        }
        await context.Reactions.AddRangeAsync(reactions);

        // 9. Seed Connections (30)
        var connections = new List<Connection>();
        for (int i = 0; i < 30; i++)
        {
            var conn = new Connection
            {
                Id = Guid.NewGuid(),
                RequesterId = users[i % users.Count].Id,
                ReceiverId = users[(i + 5) % users.Count].Id,
                Status = ConnectionStatus.Pending, // Accepted
                CreatedAtUtc = DateTime.UtcNow.AddDays(-i)
            };
            connections.Add(conn);
        }
        await context.Connections.AddRangeAsync(connections);

        // 10. Seed DirectMessages (30)
        var messages = new List<DirectMessage>();
        for (int i = 0; i < 30; i++)
        {
            var dm = new DirectMessage
            {
                Id = Guid.NewGuid(),
                SenderId = users[i % users.Count].Id,
                ReceiverId = users[(i + 3) % users.Count].Id,
                Content = $"Hey! Reaching out regarding project collaboration #{i + 1}.",
                SentAtUtc = DateTime.UtcNow.AddMinutes(-i * 15),
                IsRead = i % 2 == 0
            };
            messages.Add(dm);
        }
        await context.DirectMessages.AddRangeAsync(messages);

        // 11. Seed Notifications (30)
        var notifications = new List<Notification>();
        for (int i = 0; i < 30; i++)
        {
            var notif = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = users[i % users.Count].Id,
                Message = $"You have a new interaction on item #{i + 1}.",
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-i)
            };
            notifications.Add(notif);
        }
        await context.Notifications.AddRangeAsync(notifications);

        // 12. Seed InstitutionalEvents (30)
        var events = new List<InstitutionalEvent>();
        var adminUser = users.First(u => u.UserType == UniConnect.Domain.Enums.UserType.Admin);
        for (int i = 0; i < 30; i++)
        {
            var ev = new InstitutionalEvent
            {
                Id = Guid.NewGuid(),
                Title = $"UniConnect Campus Event #{i + 1}",
                Description = $"Join us for interactive technology workshop #{i + 1}.",
                EventDate = DateTime.UtcNow.AddDays(i + 1),
                CreatedByAdminId = adminUser.Id,
                IsPublished = true
            };
            events.Add(ev);
        }
        await context.InstitutionalEvents.AddRangeAsync(events);

        // 13. Seed Announcements (30)
        var announcements = new List<Announcement>();
        for (int i = 0; i < 30; i++)
        {
            var ann = new Announcement
            {
                Id = Guid.NewGuid(),
                Title = $"Campus Announcement #{i + 1}",
                Content = $"Important updates regarding platform features and exams #{i + 1}.",
                TargetAudience = 0,
                BroadcastAtUtc = DateTime.UtcNow.AddDays(-i)
            };
            announcements.Add(ann);
        }
        await context.Announcements.AddRangeAsync(announcements);

        // 14. Seed Experiences (30)
        var experiences = new List<Experience>();
        for (int i = 0; i < 30; i++)
        {
            var exp = new Experience
            {
                Id = Guid.NewGuid(),
                UserProfileId = userProfiles[i % userProfiles.Count].Id,
                Title = $"Software Engineer Intern #{i + 1}",
                Company = $"Enterprise Solutions {i + 1}",
                StartDate = DateTime.UtcNow.AddYears(-2),
                EndDate = DateTime.UtcNow.AddYears(-1),
                IsCurrent = false
            };
            experiences.Add(exp);
        }
        await context.Experiences.AddRangeAsync(experiences);

        // 15. Seed Certifications (30)
        var certifications = new List<Certification>();
        for (int i = 0; i < 30; i++)
        {
            var cert = new Certification
            {
                Id = Guid.NewGuid(),
                UserProfileId = userProfiles[i % userProfiles.Count].Id,
                Name = $"AWS Cloud Practitioner #{i + 1}",
                IssuingOrganization = "Amazon Web Services",
                IssueDate = DateTime.UtcNow.AddMonths(-i),
                CredentialUrl = $"https://aws.amazon.com/verify/cert_{i + 1}"
            };
            certifications.Add(cert);
        }
        await context.Certifications.AddRangeAsync(certifications);

        // 16. Seed Recommendations (30)
        var recommendations = new List<Recommendation>();
        for (int i = 0; i < 30; i++)
        {
            var rec = new Recommendation
            {
                Id = Guid.NewGuid(),
                AuthorProfileId = userProfiles[i % userProfiles.Count].Id,
                TargetProfileId = userProfiles[(i + 1) % userProfiles.Count].Id,
                Content = $"Highly recommend working with this engineer on complex backend projects #{i + 1}.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-i)
            };
            recommendations.Add(rec);
        }
        await context.Recommendations.AddRangeAsync(recommendations);

        // 17. Seed UserSkills (30)
        var userSkills = new List<UserSkill>();
        for (int i = 0; i < 30; i++)
        {
            var us = new UserSkill
            {
                UserProfileId = userProfiles[i % userProfiles.Count].Id,
                SkillId = skills[i % skills.Count].Id
            };
            if (!userSkills.Any(x => x.UserProfileId == us.UserProfileId && x.SkillId == us.SkillId))
            {
                userSkills.Add(us);
            }
        }
        await context.UserSkills.AddRangeAsync(userSkills);
        await context.SaveChangesAsync();

        // 18. Seed SkillEndorsements (30)
        var endorsements = new List<SkillEndorsement>();
        for (int i = 0; i < 30; i++)
        {
            var targetUserSkill = userSkills[i % userSkills.Count];
            var ender = new SkillEndorsement
            {
                Id = Guid.NewGuid(),
                UserSkillUserProfileId = targetUserSkill.UserProfileId,
                UserSkillSkillId = targetUserSkill.SkillId,
                EndorsedByUserId = users[(i + 4) % users.Count].Id
            };
            endorsements.Add(ender);
        }
        await context.SkillEndorsements.AddRangeAsync(endorsements);

        // 19. Seed Opportunities (30)
        var opportunities = new List<Opportunity>();
        for (int i = 0; i < 30; i++)
        {
            var opp = new Opportunity
            {
                Id = Guid.NewGuid(),
                BusinessProfileId = businessProfiles[i % businessProfiles.Count].Id,
                Title = $"Graduate Developer Role #{i + 1}",
                Description = $"Exciting entry-level position for software development graduates #{i + 1}.",
                TargetProgramme = "BSc Computer Science",
                Status = OpportunityStatus.PendingApproval,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-i)
            };
            opportunities.Add(opp);
        }
        await context.Opportunities.AddRangeAsync(opportunities);
        await context.SaveChangesAsync();

        // 20. Seed JobApplications (30)
        var applications = new List<JobApplication>();
        for (int i = 0; i < 30; i++)
        {
            var app = new JobApplication
            {
                Id = Guid.NewGuid(),
                OpportunityId = opportunities[i % opportunities.Count].Id,
                ApplicantId = users[i % users.Count].Id,
                CvFileUrl = $"https://storage.uniconnect.ac.za/applications/app_{i + 1}.pdf",
                AppliedAtUtc = DateTime.UtcNow.AddHours(-i)
            };
            applications.Add(app);
        }
        await context.JobApplications.AddRangeAsync(applications);

        // Final commit for all foreign keys
        await context.SaveChangesAsync();
    }
}