using System.Threading;
using ConfigurationService.Persistence;
using ConfigurationService.WebSocketInterface;
using NLog;

namespace ConfigurationService
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting application");
            IWebsocket connection = new RequestFlickerService("ConfigurationService");
            IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer> requestSubscriptionManager = 
                new RequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer>(connection);

            var requestHandler = new ConfigurationRequestHandler(requestSubscriptionManager, new StatePersister());

            while (true)
            {
                Thread.Sleep(300000);
                NLog.LogManager.GetCurrentClassLogger().Info("I am still alive!(Providing some infos here...)");
            }
        }
    }
}
