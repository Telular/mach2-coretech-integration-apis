using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using TankLinkNotifierApi.Models;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.IdentityModel.Tokens;

namespace TankLinkNotifierApi
{
    
    public class ServiceBusTopicSender
    {
        private readonly TopicClient _topicClient;        
        
        private readonly ILogger _logger;

        public ServiceBusTopicSender(IConfiguration configuration, ILogger<ServiceBusTopicSender> logger)
        {
            
            _logger = logger;
            ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder(configuration["ServiceBusConnectionString"]);
            string tankEntityPath = connectionStringBuilder.EntityPath;
            connectionStringBuilder.EntityPath = "";

            Initialize_ServiceBusComponents(connectionStringBuilder.ToString(), tankEntityPath, "TANK-UTILITY-READINGS", "TANK-UTILITY-CONFIGS").GetAwaiter().GetResult();

            RetryExponential retryPolicy = new RetryExponential(minimumBackoff: TimeSpan.FromSeconds(0), maximumBackoff: TimeSpan.FromSeconds(30), maximumRetryCount: 5);

            _topicClient = new TopicClient(connectionStringBuilder.ToString(), tankEntityPath, retryPolicy);
           
        }

        public async Task SendMessage(Payload payload, IDictionary<string, object> userProperties = null)
        {
            string data = JsonConvert.SerializeObject(payload);
            Message message = new Message(Encoding.UTF8.GetBytes(data));

            if (userProperties != null)
            {
                foreach(var pair in userProperties)
                {
                    message.UserProperties.TryAdd(pair.Key, pair.Value);
                }
            }            

            try
            {
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
