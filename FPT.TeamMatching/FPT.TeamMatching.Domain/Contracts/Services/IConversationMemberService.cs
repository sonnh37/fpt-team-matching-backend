using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results;
using Task = FPT.TeamMatching.Domain.Entities.Task;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IConversationMemberService
{
    Task<List<ConversationMemberPartnerInfoResult>> GetAllConversationsByUserId(Guid userId);
}