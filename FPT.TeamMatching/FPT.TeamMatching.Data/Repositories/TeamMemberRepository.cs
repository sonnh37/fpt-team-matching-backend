using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class TeamMemberRepository : BaseRepository<TeamMember>, ITeamMemberRepository
{
    private readonly FPTMatchingDbContext _dbContext;
    private readonly ISemesterRepository _semesterRepository;

    public TeamMemberRepository(FPTMatchingDbContext dbContext, ISemesterRepository semesterRepository) :
        base(dbContext)
    {
        _dbContext = dbContext;
        _semesterRepository = semesterRepository;
    }

    // Get teammber mà user đang active
    public async Task<TeamMember?> GetTeamMemberActiveByUserId(Guid userId)
    {
        var semester = await _semesterRepository.GetUpComingSemester();
        if (semester == null) return null;
        var queryable = GetQueryable(m =>
            m.UserId == userId && m.IsDeleted == false && m.Project != null && m.Project.Leader != null &&
            m.Project.Leader.UserXRoles.Any(m => m.SemesterId == semester.Id));
        return await queryable.SingleOrDefaultAsync();
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

    public async Task<List<TeamMember>> GetMembersOfTeamByProjectId(Guid projectId)
    {
        var tm = await _dbContext.TeamMembers.Where(e => e.IsDeleted == false &&
                                                         e.ProjectId == projectId &&
                                                         e.LeaveDate == null)
                                            .Include(e => e.User)
                                            .ToListAsync();
        return tm;
    }

    public async Task<TeamMember?> GetByUserAndProject(Guid userId, Guid projectId)
    {
        return await _dbContext.TeamMembers
            .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.ProjectId == projectId);
    }

    public Task<bool> UserHasTeamNow(Guid userId)
    {
        var hasTeam = GetQueryable(m => m.IsDeleted == false &&
                                        m.UserId == userId &&
                                        m.LeaveDate == null &&
                                        (m.Status == Domain.Enums.TeamMemberStatus.InProgress ||
                                         m.Status == Domain.Enums.TeamMemberStatus.Pending))
            .AnyAsync();

        return hasTeam;
    }

    public async Task<TeamMember?> GetPendingTeamMemberOfUser(Guid userId)
    {
        var teamMember = await GetQueryable(m => m.IsDeleted == false &&
                                        m.UserId == userId &&
                                        m.LeaveDate == null &&
                                        m.Status == Domain.Enums.TeamMemberStatus.Pending)
                        .FirstOrDefaultAsync();

        return teamMember;
    }
}