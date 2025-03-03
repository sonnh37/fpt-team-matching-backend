using FPT.TeamMatching.Domain.Models.Requests.Queries.Message;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IMessageService
{
    Task<BusinessResult> GetAllMessageInDay(Guid conversationId);
    Task<BusinessResult> GetMessageByConversationId(MessageGetAllQuery query);
}