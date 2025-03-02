using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IConversationMemberService
{
    Task<BusinessResult> GetAllConversationsByUserId(Guid userId);
}