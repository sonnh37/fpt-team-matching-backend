using System.Text.Json;
using AutoMapper;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Quartz.Util;
using StackExchange.Redis;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.API.Hub;

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IMapper _mapper;
    private readonly IDatabase _redis;
    private readonly IMongoUnitOfWork _unitOfWork;

    public ChatHub(IMongoUnitOfWork unitOfWork, IMapper mapper, RedisConfig redisConfig)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redis = redisConfig.GetConnection();
    }

    public async Task SendMessage(string message)
    {
        try
        {
            //1. Lấy value của connectionId trong redis
            var redisKey = $"chat:{Context.ConnectionId}";
            string? redisValue = await _redis.StringGetAsync(redisKey);

            //2. Kiểm tra xem redisKey có tồn tại không
            if (redisValue.IsNullOrWhiteSpace())
            {
                Console.WriteLine("Not found redis key");
                throw new Exception("Key not found");
            }

            //3. Convert lại redis value
            var conn = JsonSerializer.Deserialize<ConversationMemberModel>(redisValue);
            //4. Lưu message vào redis

            //5. Send message
            await Clients.Group(conn.ConversationId.ToString())
                .SendAsync("ReceiveSpecificMessage", conn.UserId, message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task JoinSpecificChatRoom(ConversationMemberModel conn)
    {
        try
        {
            //1.1 Kiểm tra xem ConversationId có empty không
            // nếu empty có nghĩa conversation đó chưa tồn tại và cần phải gửi kèm với partnerId
            // để tạo ra conversation và conversation member
            if (conn.ConversationId == null)
            {
                // Tạo conversation mới
                var conversationEntity = new Conversation
                {
                    ConversationName = ""
                };
                _unitOfWork.ConversationRepository.Add(conversationEntity);

                // Tạo conversation member
                var conversationUser = new ConversationMember
                {
                    ConversationId = conversationEntity.Id,
                    UserId = conn.UserId.ToString()
                };
                _unitOfWork.ConversationMemberRepository.Add(conversationUser);

                var conversationPartner = new ConversationMember
                {
                    ConversationId = conversationEntity.Id,
                    UserId = conn.PartnerId.ToString()
                };
                _unitOfWork.ConversationMemberRepository.Add(conversationPartner);
                await _unitOfWork.SaveChanges();
                // Gắn conversation ID vào cho request lại
                conn.ConversationId = Guid.Parse(conversationEntity.Id);
            }

            //1.2 Thêm vào SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ConversationId.ToString());

            //2. Lưu vào redis ConnectionId, JSON ConversationMemberModel
            var redisKey = $"chat:{Context.ConnectionId}";
            var redisValue = JsonSerializer.Serialize(conn);

            await _redis.StringSetAsync(redisKey, redisValue);

            //thông báo test
            await Clients.Group(conn.ConversationId.ToString()).SendAsync("JoinSpecificChatRoom", "admin",
                $"{conn.UserId} has joined {conn.ConversationId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    //Custom lại khi ngắt kết nối thì xóa key trong redis rồi mới tiếp tục xóa như default
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            //1. Xóa key trong redis
            var redisKey = $"chat:{Context.ConnectionId}";
            await _redis.KeyDeleteAsync(redisKey);
        }
        catch (Exception e)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}