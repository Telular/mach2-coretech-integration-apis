using System;
using Common.Logging;
using System.Configuration;
using System.Threading.Tasks;

namespace TankUtilityTopicReader
{
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    public class TankUtilityTopicSubReader
    {
        private static readonly ILog _logger = LogManager.GetLogger("TankUtilityTopicReader");

        private readonly string _topicName;
        private readonly string _subscriptionName;        
        private readonly string _applicationName = "Tank Utlity Topic Reader";

        private SubscriptionClient _subscriptionClient;
        

        public TankUtilityTopicSubReader(string subscriptionName) 
        {
            _topicName = ConfigurationManager.AppSettings["TankUtilityTopic"];
            _subscriptionName = subscriptionName;

            InitializeServiceBus();
        }

            

        public TankUtilityTopicSubReader(string topicName, string subscriptionName)
        {
            _topicName = topicName;
            _subscriptionName = subscriptionName;
            

            InitializeServiceBus();
        }

        private void InitializeServiceBus()
        {
            var connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];            

            RetryExponential retryPolicy = new RetryExponential(minimumBackoff: TimeSpan.FromSeconds(0), maximumBackoff: TimeSpan.FromSeconds(30), maximumRetryCount: 5);

            _subscriptionClient = new SubscriptionClient(connectionString, _topicName, _subscriptionName, ReceiveMode.PeekLock, retryPolicy);
        }

        public void ReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that processes messages.
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        public async Task CloseMessagePumpAsync()
        {
            try
            {
                // close our communication with the MT queues and topics
                if (_subscriptionClient != null && !_subscriptionClient.IsClosedOrClosing)
                {
                    await _subscriptionClient.CloseAsync();
                }

                _logger.Info($"{_applicationName} - Message pump has been stopped.");
            }
            catch (Exception ex)
            {
                _logger.Error($"{_applicationName} {ex.Message}", ex);
            }
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var stringBuilder = new StringBuilder();

            if (!token.IsCancellationRequested)
            {
                if (message != null)
                {                    
                    var messageStr = Encoding.UTF8.GetString(message.Body);

                    var messageBody = JsonConvert.DeserializeObject<Payload>(messageStr);

                    stringBuilder.AppendLine($"Event Type: {message?.UserProperties?["EventType"]}  Tank Id:  {messageBody.TankId} Received On:  {messageBody.ReceivedOn}");

                    foreach (var key in messageBody?.Data?.Keys)
                    {
                        var aKey = $"\"{key}\"";
                        stringBuilder.AppendLine($"Key:  {aKey,-30}  Value:  {messageBody?.Data?[key]}");
                    }

                    _logger.Info($"{stringBuilder}");                    

                    await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            }                       
        }

        private async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");

            await Task.Delay(1000);
        }                

    }
}
 
