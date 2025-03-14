using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface ITeamMemberService : IBaseService
{
    Task<BusinessResult> LeaveByCurrentUser();
    Task<BusinessResult> GetTeamMemberByUserId();


}