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
            //Expected input :
            //{"service":"ConfigurationService","request":{"type":"ConfigurationSubscription", "instance":"*"}}
            var subscriptionsRequestManager = new JsonRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer>(connection);
            //{"service":"ConfigurationService","request":{"type":"ConfigurationUpdate", "update":{"Instance": "inst9", "Value": "value9", "Key": "key9"}}}
            var updateRequestManager = new JsonRequestManager<ConfigurationUpdate, ConfigurationUpdateAnswer>(connection);

            var requestHandler = new ConfigurationRequestHandler(subscriptionsRequestManager,updateRequestManager, new StatePersister());

            while (true)
            {
                Thread.Sleep(300000);
                NLog.LogManager.GetCurrentClassLogger().Info("I am still alive!(Providing some infos here...)");
            }
        }
    }
}
