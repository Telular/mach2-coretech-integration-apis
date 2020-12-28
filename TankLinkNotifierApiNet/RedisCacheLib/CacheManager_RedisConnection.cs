using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;


namespace RedisCacheLib
{
    using ConnectionHelper;
    using System.Threading;
    using Newtonsoft.Json;
    using Common.Logging;
    

    public class RedisCacheManager
    {
        #region Member Vars       

        /// <summary>
        /// The Common Logging interface
        /// </summary>  
        private static readonly ILog _logger = LogManager.GetLogger("TankLinkNotifier");

        /// <summary>
        /// The interface to the Redis cache
        /// </summary>        
        private static IRedisConnection _cache;

        /// <summary>
        /// Our singleton instance
        /// </summary>
        private static RedisCacheManager _cacheManager;

        /// <summary>
        /// Inserts can tie up a connection.  So create a queue to be used as a ring buffer for connections.
        /// Our Blocking Collection of connections
        /// </summary>
        private static BlockingCollection<IRedisConnection> _databases;

        /// <summary>
        /// Number of retries on Redis Cache Commands.
        /// </summary>
        readonly int _redisCacheRetries = 5;

        /// <summary>
        /// Number of milliseconds to delay between Redis cache command retries.
        /// </summary>
        readonly int _redisCacheRetryTimeDelay = 50;

        /// <summary>
        /// Number of days (30) before cache entries expire.
        /// </summary>
        readonly int _redisCacheDefaultCacheTimeout = 30;

        /// <summary>
        /// lock objects
        /// </summary>
        private static readonly object _queueLock = new object();

        private static readonly object _singletonLock = new object();


        /// <summary>
        /// The connection to the cache used for long running background tasks like bulk inserts. Created using credentials from the app config.
        /// </summary>   
        private List<IRedisConnection> GetLazyConnectionsForInserts(int numConnections = 5)
        {
            List<IRedisConnection> returnValue = new List<IRedisConnection>();
            
            for (int i = 1; i <= numConnections; i++)
            {                                            
                returnValue.Add(new RedisConnection($"cacheConnection_{i}"));
            }

            return returnValue;
        }


        #endregion

        #region Ctor

        /// <summary>
        /// Constructor is private for this singleton instance.
        /// </summary>
        private RedisCacheManager()
        {
            SetMinimumThreads();

            int degreeOfParallelism = int.TryParse(ConfigurationManager.AppSettings["DegreeOfParallelism"], out int outResult) ? outResult : 5;

            _redisCacheRetries = int.TryParse(ConfigurationManager.AppSettings["RedisCacheRetries"], out int retryResult) ? retryResult : 5;

            _redisCacheRetryTimeDelay = int.TryParse(ConfigurationManager.AppSettings["RedisCacheRetryDelay"], out int retryTimeDelayResult) ? retryTimeDelayResult : 50;

            _redisCacheDefaultCacheTimeout = int.TryParse(ConfigurationManager.AppSettings["LongTermCacheTimeout"], out var longTermCacheTimeoutResult) ? longTermCacheTimeoutResult : 30;           


            // Build the list of "lazy" inser connection objects.
            var lazyInsertConnectionList = GetLazyConnectionsForInserts(degreeOfParallelism);
            // Populate a ConcurrentQueue with our connection list.
            var concurrentCollection = new ConcurrentQueue<IRedisConnection>(lazyInsertConnectionList);
            // Wrap our ConcurrentQueue in a Blocking Collection class that handles blocking if the queue is empty on a dequeue operation etc. (doesn't throw exceptions).
            _databases = new BlockingCollection<IRedisConnection>(concurrentCollection);

            // init our "main" cache connection which techinically isn't used anywhere...lol.
            _cache = GetLazyMainConnection();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Access to the redis connection used for operations not run in the background
        /// </summary>
        private IRedisConnection GetLazyMainConnection()
        {                        
            return new RedisConnection($"cacheConnection_main");
        }

        /// <summary>
        /// Access to the singleton instance
        /// </summary>
        public static RedisCacheManager Instance
        {
            get
            {
                if (_cacheManager == null)
                {
                    lock (_singletonLock)
                    {
                        if (_cacheManager == null)
                        {                           
                            _cacheManager = new RedisCacheManager();
                        }
                    }
                }

                return _cacheManager;
            }
        }

        #endregion

        #region Public Methods

        private class RedisCacheObject
        {
            public string Obj;
            public DateTime ExpiresOn;
        }

        public T GetFromCache<T>(string key)
        {

            // need to retry this in case its a transient error.
            var stringValue = (string)null;            

            stringValue = WhileException<string>((iDb) => iDb.StringGet(key));            

            if (string.IsNullOrEmpty(stringValue)) return default;

            try
            {
                var cachedObject = JsonConvert.DeserializeObject<RedisCacheObject>(stringValue);
                if (cachedObject.ExpiresOn <= DateTime.UtcNow)
                {
                    DeleteFromCache(key);
                    return default;
                }

                var returnObject = JsonConvert.DeserializeObject<T>(cachedObject.Obj);
                return returnObject;
            }
            catch (Exception anException)
            {
                _logger.Error($"Redis Cache Mamager - Get From Cache Error:  {anException.Message} {anException?.InnerException?.Message}");
                return default;
            }
        }

        public void AddToCache(string key, object obj, TimeSpan expiresIn)
        {
            var expiresOn = DateTime.UtcNow.AddSeconds(expiresIn.TotalSeconds);
            var cacheObject = new RedisCacheObject
            {
                Obj = JsonConvert.SerializeObject(obj),
                ExpiresOn = expiresOn
            };

            var cacheString = JsonConvert.SerializeObject(cacheObject);
            WhileException<bool>((iDb) => iDb.StringSet(key, cacheString, expiresIn));

        }

        public void AddToCache(string key, object obj)
        {            
            AddToCache(key, obj, new TimeSpan(_redisCacheDefaultCacheTimeout, 0, 0, 0));
        }

        public void DeleteFromCache(string key)
        {
            WhileException<bool>((iDb) => iDb.KeyDelete(key));            
        }

        public void PurgeAllCache()
        {
            EndPoint[] endpoints = WhileException<EndPoint[]>((iDb) => iDb.Multiplexer.GetEndPoints());            

            foreach (EndPoint endpoint in endpoints)
            {
                var iServer = WhileException<IServer>((iDb) => iDb.Multiplexer.GetServer(endpoint));
                iServer.FlushDatabase();                
            }

        }

        /// <summary>
        /// Get the status of the main multiplexer connection
        /// </summary>
        /// <returns>The parsed status as a collection of strings</returns>     
        public IEnumerable<string> GetLazyMainConnectionStatus()
        {
            return ParseStatusString(_cache.Connection.Value.GetStatus(), _cache.Connection.Value.ClientName);
        }

        /// <summary>
        /// Get the status of the bulk insert multiplexer connection
        /// </summary>
        /// <returns>The parsed status as a collection of strings</returns>      
        public IEnumerable<string> GetLazyInsertsConnectionStatus()
        {
            var returnValue = new List<string>();

            // Just iterate over the collection.  Don't need the actual value.
            foreach (var _ in _databases)
            {
                var multiplexer = GetNextCache().Multiplexer;
                var parsedStatus = ParseStatusString(multiplexer.GetStatus(), multiplexer.ClientName);

                returnValue.AddRange(parsedStatus);
            }

            return returnValue;
        }

        #endregion

        #region Private Helper Methods        

        /// <summary>
        /// Parse the mess that is the server status string into something readable.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private IEnumerable<string> ParseStatusString(string status, string connectionName)
        {
            List<string> lines = new List<string>
            {
                $"Connection: {connectionName}"
            };

            string[] parts = status.Split(',').Distinct<string>().ToArray();

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();

                if (part.StartsWith("qu="))
                {
                    lines.Add("Unsent commands (not yet on outbound network): " + part.Substring(3));
                }
                else if (part.StartsWith("qs="))
                {
                    lines.Add("Sent and awaiting response commands: " + part.Substring(3));
                }              
            }            

            return lines;
        }

        /// <summary>
        /// Get an IDatabase interface to interact with the Redis cache.
        /// </summary>
        /// <returns></returns>
        private IDatabase GetNextCache()
        {

            IDatabase aDatabase = null;

            // Get a redis connection object off the queue.
            var redisConnection = GetNextRedisConnection();

            try
            {
                aDatabase = redisConnection.Connection.Value.GetDatabase();

                aDatabase.Ping();
            }
            catch (Exception ex) when (ex is RedisConnectionException || ex is System.Net.Sockets.SocketException)
            {
                redisConnection.ForceReconnect();
                aDatabase = redisConnection.Connection.Value.GetDatabase();
            }
            catch (ObjectDisposedException)
            {
                //Ignore it's possible another reference to this redis connection is attempting an operation while we're trying to reconnect.                  
            }
            catch (NullReferenceException)
            {
                //Ignore due to Stackexchange.Redis bug https://github.com/StackExchange/StackExchange.Redis/issues/424
            }

            // Return an IDatabase interface.
            return aDatabase;

        }

        /// <summary>
        /// Get the next connection to be used for inserts
        /// </summary>
        /// <returns></returns>
        private IRedisConnection GetNextRedisConnection()
        {
            // Atomic take-add sequence on our "ring" buffer.
            lock (_queueLock)
            {               
                // Take an item off the top of the queue.
                IRedisConnection redisConnection = _databases.Take();

                // Add an item to the bottom
                _databases.Add(redisConnection);

                // Return an IDatabase interface.
                return redisConnection;
            }

        }

        // This is to handle Redis Cache commands against a database which we can reset and retry as appropriate.
        // Handles Redis cache commands that do NOT return a value.
        public void WhileException(Action<IDatabase> operation, int retries = 0, int delayMilliseconds = 0)
        {

            // Only override the passed in value if it equals "zero" i.e. no values passed in.
            // The defaults from the app.config are retries = 5, retry delay = 50 ms.
            retries = retries == 0 ? _redisCacheRetries : retries;
            delayMilliseconds = delayMilliseconds == 0 ? _redisCacheRetryTimeDelay : delayMilliseconds;

            // Get a redis connection object off the queue.
            var redisConnection = GetNextRedisConnection();


            IDatabase aDatabase;
            // Do all but one retry in the loop
            for (var retry = 1; retry < retries; retry++)
            {
                try
                {
                    aDatabase = redisConnection.Connection.Value.GetDatabase();

                    operation(aDatabase);

                    return;
                }
                catch (Exception ex) when (ex is RedisConnectionException || ex is System.Net.Sockets.SocketException)
                {
                    var forcedToReconnect = redisConnection.ForceReconnect();
                    aDatabase = redisConnection.Connection.Value.GetDatabase();

                    // if we had to "force reconnect" the Connection Multiplexer then skip the retry delay - don't need it.
                    if (forcedToReconnect)
                    {
                        continue;
                    }

                }
                catch (ObjectDisposedException)
                {
                    //Ignore it's possible another reference to this redis connection is attempting an operation while we're trying to reconnect.                  
                }
                catch (NullReferenceException)
                {
                    //Ignore due to Stackexchange.Redis bug https://github.com/StackExchange/StackExchange.Redis/issues/424
                }

                // if we didn't have to "force reconnect" the Connection Multiplexer then delay making the next retry.
                Thread.Sleep(delayMilliseconds);

            }

            // Try the operation one last time. This may or may not succeed.
            // Exceptions pass unchanged. If this is an expected exception we need to know about it because
            // we're out of retries. If it's unexpected, throwing is the right thing to do anyway
            aDatabase = redisConnection.Connection.Value.GetDatabase();

            operation(aDatabase);
        }

        // This is to handle most Redis Cache commands against a database which we can reset and retry as appropriate.
        // Handles Redis cache commands that return a value.
        public T WhileException<T>(Func<IDatabase, T> operation, int retries = 0, int delayMilliseconds = 0)
        {

            // Only override the passed in value if it equals "zero" i.e. no values passed in.
            // The defaults from the app.config are retries = 5, retry delay = 50 ms.
            retries = retries == 0 ? _redisCacheRetries : retries;
            delayMilliseconds = delayMilliseconds == 0 ? _redisCacheRetryTimeDelay : delayMilliseconds;

            // Get a redis connection object off the queue.
            var redisConnection = GetNextRedisConnection();

            IDatabase aDatabase;
            // Do all but one retry in the loop
            for (var retry = 1; retry < retries; retry++)
            {
                try
                {
                    aDatabase = redisConnection.Connection.Value.GetDatabase();


                    return operation(aDatabase);
                }
                catch (Exception ex) when (ex is RedisConnectionException || ex is System.Net.Sockets.SocketException)
                {
                    var forcedToReconnect = redisConnection.ForceReconnect();
                    aDatabase = redisConnection.Connection.Value.GetDatabase();

                    // if we had to "force reconnect" the Connection Multiplexer then SKIP delay-ing the next retry.
                    if (forcedToReconnect)
                    {
                        continue;
                    }
                }
                catch (ObjectDisposedException)
                {
                    //Ignore it's possible another reference to this redis connection is attempting an operation while we're trying to reconnect.                  
                }
                catch (NullReferenceException)
                {
                    //Ignore due to Stackexchange.Redis bug https://github.com/StackExchange/StackExchange.Redis/issues/424
                }

                // delay making the next retry.
                Thread.Sleep(delayMilliseconds);
            }

            // Try the operation one last time. This may or may not succeed.
            // Exceptions pass unchanged. If this is an expected exception we need to know about it because
            // we're out of retries. If it's unexpected, throwing is the right thing to do anyway.
            aDatabase = redisConnection.Connection.Value.GetDatabase();

            return operation(aDatabase);
        }



        /// <summary>
        /// Set the minimum worker and IOCP threads to keep alive for this app's threadpool.
        /// </summary>
        /// <returns></returns>
        private void SetMinimumThreads()
        {
            int minWorkerThreadSetting = int.TryParse(ConfigurationManager.AppSettings["MinWorkerThreads"], out int outMinWorkerThreads) ? outMinWorkerThreads : 64;
            int minIOCPThreadSetting = int.TryParse(ConfigurationManager.AppSettings["MinIocpThreads"], out int outMinIOCPThreads) ? outMinIOCPThreads : 64;

            // Get the current settings for how many "stay alive" worker and IOCP threads are currently configured for the app.
            // These are NOT per core settings but total.
            ThreadPool.GetMinThreads(out int outCurrentWorkerThreads, out int outCurrentIocpThreads);
           
            // Get the maximum limit of worker and IOCP threads that are allowed to "stay alive" at a "minimum".
            ThreadPool.GetMaxThreads(out int outMaxWorkerThreads, out int outMaxIocpThreads);

            bool checkMinWorkerThreads = ((minWorkerThreadSetting < outMaxWorkerThreads) && (minWorkerThreadSetting != outCurrentWorkerThreads));
            bool checkMinIocpThreads = ((minIOCPThreadSetting < outMaxIocpThreads) && (minIOCPThreadSetting != outCurrentIocpThreads));

          
            if (checkMinWorkerThreads || checkMinIocpThreads)
            {
                var minimumWorkerThreads = checkMinWorkerThreads ? minWorkerThreadSetting : outCurrentWorkerThreads;
                var minimumIocpThreads = checkMinIocpThreads ? minIOCPThreadSetting : outCurrentIocpThreads;

                // NOTE:  This is not a PER core setting.
                ThreadPool.SetMinThreads(minimumWorkerThreads, minimumIocpThreads);
            }
        }

        #endregion

    }


}
