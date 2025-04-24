using StackExchange.Redis;

namespace FPT.TeamMatching.Domain.Configs;

public class RedisConfig
{
    private readonly ConnectionMultiplexer _conn;

    public RedisConfig()
    {
        var conf = new ConfigurationOptions
        {
            // EndPoints = { "localhost:6379" }
            EndPoints = { "fpt.matching.redis:6379" }
        };
        _conn = ConnectionMultiplexer.Connect(conf);
    }

    public IDatabase GetConnection()
    {
        return _conn.GetDatabase();
    }

    public void CloseConnection()
    {
        _conn.Close();
    }
}