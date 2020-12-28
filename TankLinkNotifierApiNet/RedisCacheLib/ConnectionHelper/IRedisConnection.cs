using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisCacheLib.ConnectionHelper
{
    public interface IRedisConnection
    {
        DateTimeOffset LastReconnectTime { get; }
        TimeSpan ReconnectMinimumFrequency { get; }
        TimeSpan ReconnectErrorThreshold { get; }
        DateTimeOffset InitialErrorTime { get; }
        DateTimeOffset PreviousErrorTime { get; }
        Lazy<IConnectionMultiplexer> Connection { get; }       
        bool ForceReconnect();
    }
}

