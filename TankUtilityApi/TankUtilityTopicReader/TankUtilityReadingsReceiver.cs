using System;
using Common.Logging;
using System.Configuration;
using System.Threading.Tasks;

namespace TankUtilityTopicReader
{
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;   
    using System.Text;
    using System.Threading;

    public class TankUtilityTopicSubReader
    {
        private static readonly ILog _logger = LogManager.GetLogger("TankUtilityTopicReader");

        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly CancellationToken _cancellationToken;
        
        private readonly string _applicationName = "Tank Utlity Topic Reader";

        // Prefetch should be larger than the max messages - more efficient.
        private readonly int _prefetchCount = 20;
        private readonly int _maxMessagesPerRead = 10;

        private MessageReceiver _messageReceiver;
        

        public TankUtilityTopicSubReader(string subscriptionName, CancellationToken cancellationToken) 
        {
            _topicName = ConfigurationManager.AppSettings["TankUtilityTopic"];
            _subscriptionName = subscriptionName;
            _cancellationToken = cancellationToken;
            _cancellationToken.Register(async () => await CloseMessagePumpAsync());

            InitializeServiceBus();
        }

            

        public TankUtilityTopicSubReader(string topicName, string subscriptionName, CancellationToken cancellationToken)
        {
            _topicName = topicName;
            _subscriptionName = subscriptionName;

            _cancellationToken = cancellationToken;
            _cancellationToken.Register(async () => await CloseMessagePumpAsync());

            InitializeServiceBus();
        }

        private void InitializeServiceBus()
        {
            var connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];            

            RetryExponential retryPolicy = new RetryExponential(minimumBackoff: TimeSpan.FromSeconds(0), maximumBackoff: TimeSpan.FromSeconds(30), maximumRetryCount: 5);

            var entityPath = $"{_topicName}/Subscriptions/{_subscriptionName}";

            _messageReceiver = new MessageReceiver(connectionString, entityPath, ReceiveMode.PeekLock, retryPolicy, _prefetchCount);
        }

        public async Task ReceiveMessagesLoopAsync()
        {
            _logger.Info($"{_applicationName} - Message pump is running.");

            var payloadList = new List<Payload>();            

            var lockObject = new Object();

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var messageList = await _messageReceiver.ReceiveAsync(_maxMessagesPerRead, TimeSpan.FromSeconds(5));

                    // if no messages then just wait five seconds.  in reality this shoule be more like 5 minutes for production purposes.
                    if (messageList == null)
                    {

                        await Task.Delay(5000);

                        continue;
                    }

                    foreach (var message in messageList)
                    {

                        var aPayload = JsonConvert.DeserializeObject<Payload>(Encoding.UTF8.GetString(message.Body));

                        aPayload.Data.Add("EventType", message?.UserProperties?["EventType"]);                                              

                        payloadList.Add(aPayload);

                        await _messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

                    }



                    // for this non-concurrent collection just lock it and clear it.
                    lock (payloadList)
                    {
                        var sortedList = payloadList.OrderBy(o => o.Data["time"]).ToList();
                        payloadList.Clear();

                        PrintPayloads(sortedList);
                    }                    
                }
                catch(ServiceBusException sbException)
                {
                    if (!sbException.IsTransient)
                    {
                        if (!_cancellationToken.IsCancellationRequested)
                        {
                            _logger.Error($"{_applicationName} - Service Bus Non-Transient Exception.", sbException);
                            throw;
                        }
                        
                    }                    
                }
                catch(Exception anException)
                {
                    _logger.Error($"{_applicationName} - Service Bus Transient Exception.", anException);
                }
                
            }


        }

        public void PrintPayloads(List<Payload> payloadList)
        {            

            foreach (var payload in payloadList)
            {                
                PrintPayload(payload);
            }

        }

        private void PrintPayload(Payload payload)
        {
            var stringBuilder = new StringBuilder();
            Dictionary<string, object> telemetry;

            stringBuilder.AppendLine($"Tank Id:  {payload?.TankId}  Event Timestamp:  {((DateTime?)payload?.Data?["time"])?.ToString("o")}  Received On:   {payload?.ReceivedOn?.ToString("o")}");

            foreach (var key in payload?.Data?.Keys)
            {
                var aKey = $"\"{key}\"";
                stringBuilder.AppendLine($"Key:  {aKey,-30}  Value:  {payload?.Data?[key]}");
            }

            try
            {
                telemetry = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload.Data["telemetry"].ToString());

                if (telemetry.Any())
                {
                    stringBuilder.AppendLine("Telemetry:");
                }

                foreach (var key in telemetry?.Keys)
                {
                    var aKey = $"\"{key}\"";
                    stringBuilder.AppendLine($"Key:  {aKey,-30}  Value:  {payload?.Data?[key]}");
                }
            }
            catch(Exception anException)
            {
                _logger.Error($"Unable to retrieve telemetry data.  {anException.Message} {anException?.InnerException?.Message}");
            }
            
            _logger.Info($"{stringBuilder}");
        }

        public async Task CloseMessagePumpAsync()
        {
            try
            {
                // close our communication with the MT queues and topics
                if (_messageReceiver != null && !_messageReceiver.IsClosedOrClosing)
                {
                    await _messageReceiver.CloseAsync();
                }

                _logger.Info($"{_applicationName} - Message pump has been stopped.");
            }
            catch
            {
                // Ignore exceptions we're shutting down.
            }
        }            

    }
}
 
