using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Utilities.Redis;
using StackExchange.Redis;

namespace FPT.TeamMatching.Services;

public class ConversationMemberService : IConversationMemberService
{
    private readonly IMongoUnitOfWork _mongoUnitOfWork;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDatabase _redis;
    private readonly RedisUtil _redisUtil;

    public ConversationMemberService(IMongoUnitOfWork mongoUnitOfWork, IUnitOfWork unitOfWork, RedisConfig redis, RedisUtil redisUtil)
    {
        _mongoUnitOfWork = mongoUnitOfWork;
        _unitOfWork = unitOfWork;
        _redis = redis.GetConnection();
        _redisUtil = redisUtil;
    }
    
    public async Task<BusinessResult> GetAllConversationsByUserId(Guid userId)
    {
        try
        {
            //1. Lấy list conversation member ( là những parter của userId )
            var partner = await _mongoUnitOfWork.ConversationMemberRepository.GetAllByUserIdAsync(userId);
           
            //2. Call repo Lấy info
            var users = await _unitOfWork.UserRepository.GetAllUsersWithNameOnly();
            var result = new List<ConversationMemberPartnerInfoResult>();
            
            foreach (var part in partner)
            {
                var user = users.FirstOrDefault(user => user.Id.ToString() == part.UserId);
    
                if (user != null)
                {
                    //4. Lấy tin nhắn cuối cùng được lưu trong redis phục vụ hiển thị danh sách hội thoại
                    var lastedMessage = await _redisUtil.GetLastMessageFromRedis(Guid.Parse(part.ConversationId));
                    result.Add(new ConversationMemberPartnerInfoResult
                    {
                        Id = part.Id,
                        ConversationId = part.ConversationId,
                        PartnerInfoResults = new PartnerInfoResult
                        {
                            Id = part.UserId,
                            FirstName = user.FirstName, 
                            LastName = user.LastName    
                        },
                        LastMessageResult = lastedMessage,
                    });
                }
            };

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}