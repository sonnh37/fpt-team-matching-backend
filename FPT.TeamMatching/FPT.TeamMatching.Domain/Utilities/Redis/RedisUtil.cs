using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Models;
using StackExchange.Redis;

namespace FPT.TeamMatching.Domain.Utilities.Redis;

public class RedisUtil
{
    private readonly IDatabase _redis;

    public RedisUtil(RedisConfig reids)
    {
        _redis = reids.GetConnection();
    }
    
    public async Task<LastMessageResult?> GetLastMessageFromRedis(Guid conversationId)
    {
        string redisKey = $"conversation:last_message:{conversationId}";
        var lastMessageData = await _redis.HashGetAllAsync(redisKey);

        if (lastMessageData.Length == 0) return null;

        return new LastMessageResult
        {
            SenderId = Guid.Parse(lastMessageData.FirstOrDefault(x => x.Name == "senderId").Value),
            Content = lastMessageData.FirstOrDefault(x => x.Name == "content").Value,
            CreatedDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(lastMessageData.FirstOrDefault(x => x.Name == "timestamp").Value)).UtcDateTime,
            IsSeen = lastMessageData.FirstOrDefault(x => x.Name == "isSeen").Value == "1",
        };
    }
    
    public async Task SaveLastMessageToRedis(Guid? conversationId, Guid? senderId, string content, long timestamp, string isSeen)
    {
        string redisKey = $"conversation:last_message:{conversationId}";

        var lastMessageData = new HashEntry[]
        {
            new HashEntry("senderId", senderId.ToString()),
            new HashEntry("content", content),
            new HashEntry("timestamp", timestamp.ToString()),
            new HashEntry("isSeen", isSeen
            ),
        };

        await _redis.HashSetAsync(redisKey, lastMessageData);
    }
}