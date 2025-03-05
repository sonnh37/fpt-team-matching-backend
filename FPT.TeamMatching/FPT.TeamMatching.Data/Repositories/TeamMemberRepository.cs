using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class TeamMemberRepository : BaseRepository<TeamMember>, ITeamMemberRepository
{
    public TeamMemberRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
    }

    // Get teammber mà user đang active
    public Task<TeamMember?> GetTeamMemberActiveByUserId(Guid userId)
    {
        var queryable = GetQueryable(m => m.UserId == userId && m.IsDeleted == false);
        return queryable.FirstOrDefaultAsync();
    }
}