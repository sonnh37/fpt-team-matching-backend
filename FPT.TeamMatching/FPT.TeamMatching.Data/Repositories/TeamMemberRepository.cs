using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class TeamMemberRepository : BaseRepository<TeamMember>, ITeamMemberRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    public TeamMemberRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    // Get teammber mà user đang active
    public async Task<TeamMember?> GetTeamMemberActiveByUserId(Guid userId)
    {
        var queryable = GetQueryable(m => m.UserId == userId && m.IsDeleted == false);
        return await queryable.FirstOrDefaultAsync();
    }

    public async Task<List<TeamMember>> GetTeamMemberByUserId(Guid userId)
    {
        var teamMembers = await _dbContext.TeamMembers.Where(e => e.UserId == userId 
                                                        && e.IsDeleted == false)
                                                        .ToListAsync();
        return teamMembers;
    }

    public async Task<TeamMember> GetMemberByUserId(Guid userId)
    {
        var teamMember = await _dbContext.TeamMembers.SingleOrDefaultAsync(e => e.UserId == userId
                                                        && e.IsDeleted == false);
        return teamMember;
    }

    public async Task<List<TeamMember>?> GetMembersOfTeamByProjectId(Guid projectId)
    {
        var tm = await _dbContext.TeamMembers.Where(e => e.IsDeleted == false &&
                                                    e.ProjectId == projectId &&
                                                    e.LeaveDate == null)
                                                .Include(e => e.User)
                                                .ToListAsync();
        return tm;
    }
}