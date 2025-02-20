using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IInvitationService : IBaseService
{
    Task<BusinessResult> GetUserInvitationsByType(InvitationGetByTypeQuery query);
}