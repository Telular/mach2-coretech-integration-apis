using System;
using System.Net.Sockets;
using StackExchange.Redis;

namespace RedisCacheLib.ConnectionHelper
{
    class ForceReconnectSample
    {
        public static void ForceReconnect()
        {
            InitLogger();
            var connectionHelper = new ConnectionHelper();

            connectionHelper.Initialize();

            var key = "key";
            var value = "value";
            try
            {
                connectionHelper.Connection.GetDatabase().KeyDelete(key);
                connectionHelper.Connection.GetDatabase().StringSet(key, value);
                var newValue = connectionHelper.Connection.GetDatabase().StringGet(key);

                Console.WriteLine("new value is {0}, expected value is {1}", newValue, value);
            }
            catch (Exception ex) when (ex is RedisConnectionException || ex is SocketException)
            {
                connectionHelper.ForceReconnect();
            }
            catch (ObjectDisposedException)
            {
                LogUtility.LogInfo("Retry later since reconnection is in progress");
            }
            catch (NullReferenceException)
            {
                //Ignore due to Stackexchange.Redis bug https://github.com/StackExchange/StackExchange.Redis/issues/424
            }
        }

        private static void InitLogger()
        {
            LogUtility.Logger = Console.Out;
        }
    }
}
