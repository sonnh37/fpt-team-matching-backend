using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IConversationMemberRepository
{
    void Add(ConversationMember conversationMember);
    void Delete(ConversationMember conversationMember);
    Task<ConversationMember?> GetByIdAsync(Guid conversationMemberId);
    Task<List<ConversationMember>> GetAllAsync();
    Task<List<ConversationMember>> GetAllByUserIdAsync(Guid userId);
    Task<List<ConversationMember>> GetAllByConversationIdAsync(Guid teamId);
}