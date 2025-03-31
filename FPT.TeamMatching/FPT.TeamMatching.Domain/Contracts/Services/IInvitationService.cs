using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IInvitationService : IBaseService
{
    Task<BusinessResult> GetUserInvitationsByStatus(InvitationGetListForUserByStatus query);

    Task<BusinessResult> GetUserInvitationsByType(InvitationGetByTypeQuery query);
    Task<BusinessResult> GetLeaderInvitationsByType(InvitationGetByTypeQuery query);
    Task<BusinessResult> TeamCreatePending(InvitationTeamCreatePendingCommand command);
    Task<BusinessResult> StudentCreatePending(InvitationStudentCreatePendingCommand command);
    Task<BusinessResult> CheckIfStudentSendInvitationByProjectId(Guid projectId);
    Task<BusinessResult> DeletePermanentInvitation(Guid projectId);

    Task<BusinessResult> ApproveOrRejectInvitationFromTeamByMe(InvitationUpdateCommand command);
    Task<BusinessResult> ApproveOrRejectInvitationFromPersonalizeByLeader(InvitationUpdateCommand command);
}