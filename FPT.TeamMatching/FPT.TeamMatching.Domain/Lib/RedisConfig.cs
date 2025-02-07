using StackExchange.Redis;

namespace FPT.TeamMatching.Domain.Lib;

public class RedisConfig
{
    private readonly ConnectionMultiplexer _conn;

    public RedisConfig()
    {
        ConfigurationOptions conf = new ConfigurationOptions()
        {
            EndPoints = { "localhost: 6379" },
        };
        _conn = ConnectionMultiplexer.Connect(conf);
    }
    
    public IDatabase GetConnection() => _conn.GetDatabase();
    public void CloseConnection() => _conn.Close();
}