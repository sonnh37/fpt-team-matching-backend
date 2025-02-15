using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using Task = FPT.TeamMatching.Domain.Entities.Task;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IConversationMemberService
{
    Task<BusinessResult> GetAllConversationsByUserId(Guid userId);
}