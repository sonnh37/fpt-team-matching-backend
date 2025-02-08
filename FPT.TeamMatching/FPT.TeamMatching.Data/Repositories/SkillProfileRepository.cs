using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class SkillProfileRepository : BaseRepository<SkillProfile>, ISkillProfileRepository
{
    private readonly FPTMatchingDbContext _dbContext;

    public SkillProfileRepository(IMapper mapper, FPTMatchingDbContext dbContext) : base(dbContext, mapper)
    {
        _dbContext = dbContext;
    }

    public async Task<SkillProfile> GetSkillProfileByUserId(Guid userId)
    {
        return await _dbContext.SkillProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<SkillProfile> UpsertSkillProfile(SkillProfile skillProfile)
    {
        var existingSkillProfile = await _dbContext.SkillProfiles
            .FirstOrDefaultAsync(x => x.UserId == skillProfile.UserId);

        if (existingSkillProfile != null)
            // Update existing profile
            _dbContext.Entry(existingSkillProfile).CurrentValues.SetValues(skillProfile);
        else
            // Insert new profile
            await _dbContext.SkillProfiles.AddAsync(skillProfile);

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        return skillProfile;
    }
}