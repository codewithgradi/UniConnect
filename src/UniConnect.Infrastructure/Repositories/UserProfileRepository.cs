using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class UserProfileRepository : RepositoryBase<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<UserProfile?> GetProfileWithDetailsAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserProfiles
            .Include(p => p.Experiences)
            .Include(p => p.Certifications)
            .Include(p => p.UserSkills)
                .ThenInclude(us => us.Skill)
            .Include(p => p.ReceivedRecommendations)
            .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> SearchProfilesAsync(string? searchTerm, string? programme, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.UserProfiles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(p => p.FirstName.ToLower().Contains(term)
                                  || p.LastName.ToLower().Contains(term)
                                  || p.SystemHeadline.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(programme))
        {
            query = query.Where(p => p.Programme == programme);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserProfiles.AddAsync(profile, cancellationToken);
    }

    public void Update(UserProfile profile)
    {
        _dbContext.UserProfiles.Update(profile);
    }

    public async Task AddSkillToProfileAsync(Guid profileId, Guid skillId, CancellationToken cancellationToken = default)
    {
        var userSkill = new UserSkill
        {
            UserProfileId = profileId,
            SkillId = skillId
        };
        await _dbContext.UserSkills.AddAsync(userSkill, cancellationToken);
    }

    public async Task EndorseSkillAsync(Guid targetProfileId, Guid skillId, Guid endorsedByUserId, CancellationToken cancellationToken = default)
    {
        var endorsement = new SkillEndorsement
        {
            Id = Guid.NewGuid(),
            UserSkillUserProfileId = targetProfileId,
            UserSkillSkillId = skillId,
            EndorsedByUserId = endorsedByUserId
        };
        await _dbContext.SkillEndorsements.AddAsync(endorsement, cancellationToken);
    }
}