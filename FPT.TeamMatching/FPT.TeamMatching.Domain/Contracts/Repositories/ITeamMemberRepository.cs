using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface ITeamMemberRepository : IBaseRepository<TeamMember>
{
    Task<TeamMember?> GetTeamMemberActiveByUserId(Guid userId, Guid semesterId);

    Task<List<TeamMember>> GetTeamMemberByUserId(Guid userId);

    Task<TeamMember> GetMemberByUserId(Guid userId);

    //get list thanh vien hien tai cua team
    Task<List<TeamMember>> GetMembersOfTeamByProjectId(Guid projectId);

    Task<TeamMember?> GetByUserAndProject(Guid userId, Guid projectId);

    Task<bool> UserHasTeamNow(Guid userId);

    Task<TeamMember?> GetPendingTeamMemberOfUser(Guid userId);

}