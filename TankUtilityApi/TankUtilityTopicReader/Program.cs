using System;
using System.Configuration;
using System.Threading.Tasks;

namespace TankUtilityTopicReader
{
    using Common.Logging; 

    class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger("TankUtilityTopicReader");
        

        static async Task Main(string[] args)
        {
            try
            {
                
                var readingSubscriptionName = ConfigurationManager.AppSettings["ReadingsSubscription"];
                var configsSuscriptionName = ConfigurationManager.AppSettings["ConfigsSubscription"];


                Console.WriteLine("======================================================");
                Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
                Console.WriteLine("======================================================");

                var tankReadingsReciever = new TankUtilityTopicSubReader(readingSubscriptionName);

                tankReadingsReciever.ReceiveMessages();

                var deviceConfigsReceiver = new TankUtilityTopicSubReader(configsSuscriptionName);

                deviceConfigsReceiver.ReceiveMessages();

                Console.ReadKey();

                await tankReadingsReciever.CloseMessagePumpAsync();
                await deviceConfigsReceiver.CloseMessagePumpAsync();
            }
            catch (Exception anException)
            {
                _logger.Error("Exception", anException);
            }

        }
}
}
