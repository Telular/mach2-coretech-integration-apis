using System;
using System.Configuration;
using System.Threading.Tasks;

namespace TankUtilityTopicReader
{
    using Common.Logging;
    using System.Threading;

    class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger("TankUtilityTopicReader");
        

        static async Task Main(string[] args)
        {
            try
            {
                var cancellationTokenReadings = new CancellationTokenSource();                                
                
                var readingSubscriptionName = ConfigurationManager.AppSettings["ReadingsSubscription"];
                var configsSuscriptionName = ConfigurationManager.AppSettings["ConfigsSubscription"];


                Console.WriteLine("======================================================");
                Console.WriteLine("Press ENTER key to exit recieve message loop.");
                Console.WriteLine("======================================================");

                var tankReadingsReciever = new TankUtilityTopicSubReader(readingSubscriptionName, cancellationTokenReadings.Token);

                var aTask = tankReadingsReciever.ReceiveMessagesLoopAsync();

                Console.ReadKey();

                cancellationTokenReadings.Cancel();

                await Task.WhenAll(new Task[] { aTask });
                          
            }
            catch (Exception anException)
            {
                _logger.Error("Exception", anException);
            }

        }
}
}
