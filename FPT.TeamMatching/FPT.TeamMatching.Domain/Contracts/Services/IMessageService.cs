using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IMessageService
{
    Task<BusinessResult> GetAllMessageInDay(Guid conversationId);
}