using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class SkillService : ISkillService
{
    private readonly IUnitOfWork _unitOfWork;

    public SkillService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SkillDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var skills = await _unitOfWork.Skills.GetAllAsync(cancellationToken);
        return skills.Select(s => new SkillDto(s.Id, s.Name));
    }

    public async Task<SkillDto> CreateAsync(CreateSkillDto dto, CancellationToken cancellationToken = default)
    {
        var existingSkill = await _unitOfWork.Skills.GetByNameAsync(dto.Name, cancellationToken);
        if (existingSkill != null)
        {
            throw new InvalidOperationException($"Skill '{dto.Name}' already exists.");
        }

        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
        };

        await _unitOfWork.Skills.AddAsync(skill, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SkillDto(skill.Id, skill.Name);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var skill = await _unitOfWork.Skills.GetByIdAsync(id, cancellationToken);
        if (skill == null)
        {
            throw new KeyNotFoundException("Skill not found.");
        }

        _unitOfWork.Skills.Remove(skill);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}