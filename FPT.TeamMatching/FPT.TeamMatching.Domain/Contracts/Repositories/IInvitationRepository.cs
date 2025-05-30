using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IInvitationRepository : IBaseRepository<Invitation>
{
    Task<Invitation?> GetInvitationOfUserByProjectId(Guid projectId, Guid userId);
    Task<(List<Invitation>, int)> GetUserInvitationsByType(InvitationGetByTypeQuery query, Guid userId, Guid semesterId);
    Task<(List<Invitation>, int)> GetUserInvitationsByStatus(InvitationGetListForUserByStatus query, Guid userId);
    Task<(List<Invitation>, int)> GetLeaderInvitationsByType(InvitationGetByTypeQuery query, Guid userId);
    Task<Invitation?> GetInvitationOfTeamByProjectIdAndMe(Guid projectId, Guid userId);
    
    // Trong IInvitationRepository
    Task<List<Invitation>> GetPendingInvitationsForProjectFromOtherSendersAsync(Guid userId, Guid projectId, Guid excludeInvitationId);

    Task<List<Invitation>> GetInvitationsByStatusAndProjectId(InvitationStatus status, Guid projectId);
    Task<List<Invitation>?> GetInvitationsBySenderIdAndStatus(Guid senderId, InvitationStatus status);
}
