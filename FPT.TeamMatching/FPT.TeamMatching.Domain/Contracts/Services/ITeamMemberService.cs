using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface ITeamMemberService : IBaseService
{
    Task<BusinessResult> LeaveByCurrentUser();
    Task<BusinessResult> GetTeamMemberByUserId();
    Task<BusinessResult> UpdateTeamMemberByMentor(List<MentorUpdate> requests);
    Task<BusinessResult> UpdateTeamMemberByManager(ManagerUpdate requests);
    Task<BusinessResult> AddRange(TeamMemberAddRangeCommand command);
}