using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using TankNotifierApiNet.Models;
using Newtonsoft.Json;
using Common.Logging;
using System.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace TankNotifierApiNet
{
    
    public class ServiceBusTopicSender
    {
        private readonly TopicClient _topicClient;

        //private static readonly ILog _logger = LogManager.GetLogger("TankLinkNotifier");

        private static readonly object _singletonLock = new object();

        /// <summary>
        /// Our singleton instance
        /// </summary>
        private static ServiceBusTopicSender _singletonSender;

        public ServiceBusTopicSender()
        {
            
            
            ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder(ConfigurationManager.AppSettings["ServiceBusConnectionString"])
            {
                // A precaution in case the connection string has a path included.
                EntityPath = ""
            };

            string tankUtilityTopic = ConfigurationManager.AppSettings["TankUtilityTopic"];
            string tankUtilityReadingsSub = ConfigurationManager.AppSettings["ReadingsSubcription"];
            string tankUtlityConfigChangeSub = ConfigurationManager.AppSettings["ConfigChangesSubscription"];

            // This async task will NOT run correctly in the thread context for a web call unless it's expressly coded wrapped in a Task.Run statement.
            // Async calls downstream like managementClient.TopicExistsAsync would just run and never return to the current thread context. 
            var aTask = Task.Run(async () => await Initialize_ServiceBusComponents(connectionStringBuilder.ToString(), tankUtilityTopic, tankUtilityReadingsSub, tankUtlityConfigChangeSub));

            aTask.GetAwaiter().GetResult();

            RetryExponential retryPolicy = new RetryExponential(minimumBackoff: TimeSpan.FromSeconds(0), maximumBackoff: TimeSpan.FromSeconds(30), maximumRetryCount: 5);

            _topicClient = new TopicClient(connectionStringBuilder.ToString(), tankUtilityTopic, retryPolicy);
           
        }

        /// <summary>
        /// Access to the singleton instance
        /// </summary>
        public static ServiceBusTopicSender Instance
        {
            get
            {
                if (_singletonSender == null)
                {
                    lock (_singletonLock)
                    {
                        if (_singletonSender == null)
                        {
                            _singletonSender = new ServiceBusTopicSender();
                        }
                    }
                }

                return _singletonSender;
            }
        }

        public async Task SendMessage(Payload payload, IDictionary<string, object> userProperties = null)
        {
                 

            try
            {
                string data = JsonConvert.SerializeObject(payload);
                Message message = new Message(Encoding.UTF8.GetBytes(data))
                {
                    ContentType = "application/json"
                };

                if (userProperties != null)
                {
                    foreach (var pair in userProperties)
                    {
                        if (!message.UserProperties.ContainsKey(pair.Key))
                        {
                            message.UserProperties.Add(pair.Key, pair.Value);
                        }
                    }
                }

                await _topicClient.SendAsync(message);
            }
            catch (Exception anException)
            {
                throw anException;
            }
        }

        private async Task Initialize_ServiceBusComponents(string aServiceBusConnectStr, string aTopicPath, string tankReadingSubName, string tankConfigSubName)
        {            
            var managementClient = new ManagementClient(aServiceBusConnectStr);

            var topicExists = await CreateServiceBusTopic(managementClient, aTopicPath);

            if (topicExists)
            {
                await CreateServiceBusTopicSubscription(managementClient, aTopicPath, tankReadingSubName, "TU_TANK_READINGS", "EventType = 'UPDATE_TU_TANK_READING'");
                await CreateServiceBusTopicSubscription(managementClient, aTopicPath, tankConfigSubName, "TU_TANK_CONFIG_CHANGES", "EventType = 'UPDATE_TU_TANK_CONFIG'");
            }           
            
        }

        private async Task<bool> CreateServiceBusTopic(ManagementClient managementClient, string aTopicPath)
        {
            

            bool aTopicExists = await managementClient.TopicExistsAsync(aTopicPath);

            if (!aTopicExists)
            {
                var topicDescription = new TopicDescription(aTopicPath)
                {
                    AutoDeleteOnIdle = TimeSpan.FromDays(14),
                    DefaultMessageTimeToLive = TimeSpan.FromDays(7),
                    EnableBatchedOperations = true,
                    SupportOrdering = true,
                    DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10)
                };

                await managementClient.CreateTopicAsync(topicDescription);

                aTopicExists = await managementClient.TopicExistsAsync(aTopicPath);
            }


            return aTopicExists;

        }

        private async Task<bool> CreateServiceBusTopicSubscription(ManagementClient managementClient, string aTopicPath, string aTopicSubscriptionName, string aRuleDescriptionName, string sqlFilterExpression)
        {            

            bool aSubscriptionExists = await managementClient.SubscriptionExistsAsync(aTopicPath, aTopicSubscriptionName);

            if (!aSubscriptionExists)
            {
                var aSubscriptionDescription = new SubscriptionDescription(aTopicPath, aTopicSubscriptionName)
                {
                    LockDuration = TimeSpan.FromMinutes(1),
                    AutoDeleteOnIdle = TimeSpan.FromDays(14),
                    DefaultMessageTimeToLive = TimeSpan.FromDays(7),
                    EnableBatchedOperations = true,
                    EnableDeadLetteringOnFilterEvaluationExceptions = true,
                    EnableDeadLetteringOnMessageExpiration = true,
                    MaxDeliveryCount = 10
                };

                var subCreatedDesc = await managementClient.CreateSubscriptionAsync(aSubscriptionDescription);

                if (subCreatedDesc != null)
                {
                    var ruleDescription = new RuleDescription(aRuleDescriptionName, new SqlFilter(sqlFilterExpression));

                    await managementClient.CreateRuleAsync(aTopicPath, aTopicSubscriptionName, ruleDescription);

                    await managementClient.DeleteRuleAsync(aTopicPath, aTopicSubscriptionName, "$Default");
                }

                aSubscriptionExists = await managementClient.SubscriptionExistsAsync(aTopicPath, aTopicSubscriptionName);
            }
           
            return aSubscriptionExists;

        }        
    }
}
