using StackExchange.Redis;
using System;
using System.Configuration; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace RedisCacheLib.ConnectionHelper
{

    public class RedisConnection : IRedisConnection, IDisposable
    {        
        private Object _lockObject = new object();

        // Flag: Has Dispose already been called?
        bool _disposed = false;

        public RedisConnection() : this(null)
        {           
        }

        public RedisConnection(string connectionName)
        {
            // if the connection name passed in is null etc. then set the connection name to a default.
            // NOTE:  Redis doesn't require a "unique" client name on a connection so what's below will more than suffice. 
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                ConnectionName = $"cacheConnection_{DateTime.UtcNow.ToString("yyyyMMddhhmmssfff")}";
            }
            else
            {
                ConnectionName = connectionName;
            }

            SetupMultiplexerConnection();
        }

        public DateTimeOffset LastReconnectTime { get; private set; }
        public TimeSpan ReconnectMinimumFrequency { get; private set; }
        public TimeSpan ReconnectErrorThreshold { get; private set; }
        public DateTimeOffset InitialErrorTime { get; private set; }
        public DateTimeOffset PreviousErrorTime { get; private set; }
        public bool CheckRedisCertificateForErrors { get; private set; }
        public string ConnectionName { get; private set; }

        public Lazy<IConnectionMultiplexer> Connection { get; private set; }

        private void SetupMultiplexerConnection()
        {
            LastReconnectTime = DateTimeOffset.MinValue;
            PreviousErrorTime = DateTimeOffset.MinValue;
            InitialErrorTime = DateTimeOffset.MinValue;
                       
            ReconnectErrorThreshold = TimeSpan.FromSeconds(int.TryParse(ConfigurationManager.AppSettings["ReconnectErrorThreshold"], out int outErrorThreshold) ? outErrorThreshold : 30);
            ReconnectMinimumFrequency = TimeSpan.FromSeconds(int.TryParse(ConfigurationManager.AppSettings["ReconnectMinimumFrequency"], out int outMinFrequency) ? outMinFrequency : 30);

            CheckRedisCertificateForErrors = bool.TryParse(ConfigurationManager.AppSettings["CheckRedisCertForErrors"], out bool checkRedisCertForErrors) ? checkRedisCertForErrors : false;

            var configString = $"{ConfigurationManager.AppSettings["CacheConfigString"]},name={ConnectionName}";
            var redisConfiguration = ConfigurationOptions.Parse(configString, true);

            if (redisConfiguration.Ssl && CheckRedisCertificateForErrors)
            {
                redisConfiguration.CertificateValidation += ValidateServerCertificate;
                //redisConfiguration.CertificateSelection += CertificateSelection;
                //redisConfiguration.SslHost = "mach2-coretech-cache-development.redis.cache.windows.net";
            }

            Connection = CreateMultiplexer(redisConfiguration);
        }      

        private Lazy<IConnectionMultiplexer> CreateMultiplexer(ConfigurationOptions config)
        {
            return new Lazy<IConnectionMultiplexer>(() =>
            {
                var aConnection = ConnectionMultiplexer.Connect(config);

                // These are the delegate event hooks for that can be made use of on the ConnectionMultiplexer.
                aConnection.ConnectionFailed += (sender, args) =>
                {
                    // Output to a logger when a connection fails.
                };

                aConnection.ConnectionRestored += (sender, args) =>
                {
                    // Output to a logger when a connection is restored.
                };

                aConnection.ErrorMessage += (sender, args) =>
                {
                    // Output to a logger when a server responds with an error message.
                };

                //aConnection.InternalError += (sender, args) =>
                //{
                //    // Output to a logger when a multiplexer generates and internal error.
                //    // Primarily used for debugging.
                //};

                return aConnection;
            });
        }

        private void CloseMultiplexerConnection(bool allowCommandsToComplete = true)
        {
            if (Connection == null)
            {
                return;
            }

            try
            {
                Connection.Value?.Close(allowCommandsToComplete);
                Connection.Value?.Dispose();
            }
            catch
            {
                // Do nothing we're closing down the connection.
            }

        }

        public bool ForceReconnect()
        {
            bool doForceReconnect = false;

            // LastReconnectTime is initialized at startup to be DateTimeOffset.MinValue hence
            // this clause will succeed if this is the first error encountered on this connection.
            if ((DateTimeOffset.UtcNow - LastReconnectTime) > ReconnectMinimumFrequency)
            {
                lock (_lockObject)
                {
                    var utcNow = DateTimeOffset.UtcNow;

                    if (InitialErrorTime == DateTimeOffset.MinValue)
                    {
                        // We haven't seen an error since last (re-)connect, so set initial values.
                        InitialErrorTime = utcNow;
                        PreviousErrorTime = utcNow;
                        return doForceReconnect;
                    }

                    // In general, let StackExchange.Redis handle most reconnects, 
                    // so limit the frequency of how often this will actually reconnect.
                    if ((utcNow - LastReconnectTime) < ReconnectMinimumFrequency)
                    {
                        // Let the connection self-heal.
                        return doForceReconnect;
                    }

                    // if errors continue for longer than the ReconnectErrorThreshold threshold, then the 
                    // multiplexer seems to not be reconnecting, so re-create the multiplexer
                    doForceReconnect =
                        // make sure we gave the multiplexer enough time to reconnect on its own if it can
                        // make sure we aren't working on stale data (e.g. if there was a gap in errors, don't reconnect yet).  
                        ((utcNow - InitialErrorTime) >= ReconnectErrorThreshold) && ((utcNow - PreviousErrorTime) <= ReconnectErrorThreshold);

                    PreviousErrorTime = utcNow;

                    if (doForceReconnect)
                    {
                        // Close it NOW.
                        CloseMultiplexerConnection( allowCommandsToComplete: false );

                        // Initializes timestamp fields and recreates a Connection Multiplexer.
                        SetupMultiplexerConnection();

                        // Make sure to set this field LAST to indicate a "reconnect" occurred.
                        LastReconnectTime = utcNow;
                    }

                }
            }

            return doForceReconnect;
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                CloseMultiplexerConnection();
            }

            _disposed = true;
        }

        // DO NOT Throw an exception from this method.  
        // Return false, on SSL Errors, and let the Redis Connection surface the Invalid Certificate exception.
        public static bool ValidateServerCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            return false;
        }

        // Unused.
        public X509Certificate CertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            if (!(localCertificates.Count > 0))
            {
                return null;
            }

            return null;
        }
    }
}

