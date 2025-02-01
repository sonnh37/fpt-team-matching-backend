using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Profile = FPT.TeamMatching.Domain.Entities.Profile;

namespace FPT.TeamMatching.Data.Repositories;

public class ProfileRepository : BaseRepository<Profile>, IProfileRepository
{
    private readonly FPTMatchingDbContext _dbContext;   
    public ProfileRepository(IMapper mapper, FPTMatchingDbContext dbContext) : base(dbContext, mapper)
    {
        _dbContext = dbContext;
    }

    public async Task<Profile> GetProfileByUserId(Guid userId)
    {
        return await _dbContext.Profiles.FirstOrDefaultAsync(x => x.UserId == userId);
    }
}