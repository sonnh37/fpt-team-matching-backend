using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Services;

public class ConversationMemberService : IConversationMemberService
{
    private readonly IMongoUnitOfWork _mongoUnitOfWork;
    private readonly IUnitOfWork _unitOfWork;

    public ConversationMemberService(IMongoUnitOfWork mongoUnitOfWork, IUnitOfWork unitOfWork)
    {
        _mongoUnitOfWork = mongoUnitOfWork;
        _unitOfWork = unitOfWork;
    }
    public async Task<List<ConversationMemberPartnerInfoResult>> GetAllConversationsByUserId(Guid userId)
    {
        try
        {
            //1. Lấy list conversation member ( là những parter của userId )
            var partner = await _mongoUnitOfWork.ConversationMemberRepository.GetAllByUserIdAsync(userId);
           
            //2. Lấy list ids để phục vụ lấy info
            // var partnerIds = partner.Select(c => c.UserId)
            //     .ToList();
            
            //3. Call repo Lấy info
            var users = await _unitOfWork.UserRepository.GetAllUsersWithNameOnly();
            var result = new List<ConversationMemberPartnerInfoResult>();
            
            partner.ForEach(part =>
            {
                var user = users.FirstOrDefault(user => user.Id.ToString() == part.UserId);
    
                if (user != null)
                {
                    result.Add(new ConversationMemberPartnerInfoResult
                    {
                        Id = part.Id,
                        ConversationId = part.ConversationId,
                        PartnerInfoResults = new List<PartnerInfoResult>
                        {
                            new PartnerInfoResult
                            {
                                Id = part.UserId,
                                FirstName = user.FirstName, 
                                LastName = user.LastName    
                            }
                        }
                    });
                }
            });

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}