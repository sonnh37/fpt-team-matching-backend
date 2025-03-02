using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IInvitationRepository : IBaseRepository<Invitation>
{
    Task<Invitation?> GetInvitationOfUserByProjectId(Guid projectId, Guid userId);
    Task<(List<Invitation>, int)> GetUserInvitationsByType(InvitationGetByTypeQuery query, Guid userId);

}
