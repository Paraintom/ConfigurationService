using System.Collections.Generic;
using System.Linq;
using ConfigurationService.Persistence;
using ConfigurationService.WebSocketInterface;

namespace ConfigurationService
{
    public class ConfigurationRequestHandler
    {
        private IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer> subscriptionsRequest;
        private IStatePersister persister;
        private List<Configuration> allConfigurations;
        public const string ConfigurationInstanceWildcard = "*";

        public ConfigurationRequestHandler(
            IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer> subscriptionsRequest,
            IStatePersister persister)
        {
            this.subscriptionsRequest = subscriptionsRequest;
            this.persister = persister;
            this.allConfigurations = persister.Read();
            subscriptionsRequest.OnRequest += (sender, message) => OnNewRequest(message);
        }

        private void OnNewRequest(RequestMessage<ConfigurationSubscription> request)
        {
            var instanceFilter = request.request.instance;
            var matchingConfigurations =
                instanceFilter == ConfigurationInstanceWildcard ?
                allConfigurations : 
                allConfigurations.Where(o => o.Instance == instanceFilter);

            subscriptionsRequest.Send(new AnwserMessage<ConfigurationSubscriptionAnswer>()
            {
                id = request.id,
                answer = new ConfigurationSubscriptionAnswer()
                {
                    result = matchingConfigurations.ToList()
                }
            });
        }
    }
}