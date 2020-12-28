using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheLib.ConnectionHelper
{
    using System;
    using System.Configuration;
    using StackExchange.Redis;

    /// <summary>
    /// ConnectionHelper supports reconnections on RedisConnectionException.
    /// Current retry policy is fixed time interval retry, which mean two reconnect won't happen in reconnectMinFrequency
    /// </summary> 
    public class ConnectionHelper
    {
        private DateTimeOffset _lastReconnectTime = DateTimeOffset.MinValue;
        private DateTimeOffset _firstErrorTime = DateTimeOffset.MinValue;

        private DateTimeOffset _previousErrorTime = DateTimeOffset.MinValue;

        // In general, let StackExchange.Redis handle most reconnects, 
        // so limit the frequency of how often this will actually reconnect.
        public TimeSpan _reconnectMinFrequency = TimeSpan.FromSeconds(60);

        // if errors continue for longer than the below threshold, then the 
        // multiplexer seems to not be reconnecting, so re-create the multiplexer
        public TimeSpan _reconnectErrorThreshold = TimeSpan.FromSeconds(30);

        private readonly object _reconnectLock = new object();
        private ConfigurationOptions _redisConfiguration;

        private Lazy<IConnectionMultiplexer> _multiplexer;
        private bool _initialized = false;


        /// <exception cref="ObjectDisposedException">when force reconnecting.</exception>
        public IConnectionMultiplexer Connection
        {
            get
            {
                EnsureInitialized();
                return _multiplexer.Value;
            }
        }

        public void Initialize()
        {
            var configString = ConfigurationManager.AppSettings["CacheConfigString"];

            Initialize(configString);
        }

        private void Initialize(string configString)
        {
            _redisConfiguration = ConfigurationOptions.Parse(configString,true);

            _multiplexer = CreateMultiplexer(_redisConfiguration);

            _initialized = true;
        }

        /// <summary>
        /// Force a new ConnectionMultiplexer to be created.  
        /// NOTES: 
        ///     1. Users of the ConnectionMultiplexer MUST handle ObjectDisposedExceptions, which can now happen when get Connection property
        ///     2. Don't call ForceReconnect for Timeouts, just for RedisConnectionExceptions
        ///     3. Call this method every time you see a connection exception, the code will wait to reconnect:
        ///         a. for at least the "ReconnectErrorThreshold" time of repeated errors before actually reconnecting
        ///         b. not reconnect more frequently than configured in "ReconnectMinFrequency"
        /// </summary>
        public void ForceReconnect()
        {
            EnsureInitialized();
            var previousReconnect = _lastReconnectTime;
            var elapsedSinceLastReconnect = DateTimeOffset.UtcNow - previousReconnect;

            // If mulitple threads call ForceReconnect at the same time, we only want to honor one of them.
            if (elapsedSinceLastReconnect > _reconnectMinFrequency)
            {
                lock (_reconnectLock)
                {
                    var utcNow = DateTimeOffset.UtcNow;
                    elapsedSinceLastReconnect = utcNow - _lastReconnectTime;

                    if (_firstErrorTime == DateTimeOffset.MinValue)
                    {
                        // We haven't seen an error since last reconnect, so set initial values.
                        _firstErrorTime = utcNow;
                        _previousErrorTime = utcNow;
                        return;
                    }


                    // Some other thread made it through the check and the lock, so wait to next connect time.
                    if (elapsedSinceLastReconnect < _reconnectMinFrequency)
                    {
                        return;
                    }

                    var elapsedSinceFirstError = utcNow - _firstErrorTime;
                    var elapsedSinceMostRecentError = utcNow - _previousErrorTime;
                    _previousErrorTime = utcNow;

                    var shouldReconnect =
                        // make sure we gave the multiplexer enough time to reconnect on its own if it can
                        // make sure we aren't working on stale data (e.g. if there was a gap in errors, don't reconnect yet).  
                        elapsedSinceFirstError >= _reconnectErrorThreshold && elapsedSinceMostRecentError <= _reconnectErrorThreshold;

                    if (shouldReconnect)
                    {                                              
                        CloseMultiplexer(_multiplexer);
                        _multiplexer = CreateMultiplexer(_redisConfiguration);

                        _firstErrorTime = DateTimeOffset.MinValue;
                        _previousErrorTime = DateTimeOffset.MinValue;
                        _lastReconnectTime = utcNow;
                    }
                   
                }
            }
           
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                Initialize();
            }
        }

        private Lazy<IConnectionMultiplexer> CreateMultiplexer(ConfigurationOptions config)
        {
            return new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(config));
        }

        private void CloseMultiplexer(Lazy<IConnectionMultiplexer> multiplexer)
        {
            if (multiplexer == null)
            {
                return;
            }

            try
            {
                multiplexer.Value.Close();
            }
            catch
            {
                // Do nothing we're closing down the connection.
            }
           
        }
    }

}
