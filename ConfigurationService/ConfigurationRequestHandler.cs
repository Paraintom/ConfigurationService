using System.Collections.Generic;
using System.Linq;
using ConfigurationService.Persistence;
using ConfigurationService.WebSocketInterface;

namespace ConfigurationService
{
    public class ConfigurationRequestHandler
    {
        private readonly IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer> subscriptionsRequestSource;
        private IStatePersister persister;
        private List<Configuration> allConfigurations;
        public const string ConfigurationInstanceWildcard = "*";

        public ConfigurationRequestHandler(
            IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer> subscriptionsRequestSource,
            IStatePersister persister)
        {
            this.subscriptionsRequestSource = subscriptionsRequestSource;
            this.persister = persister;
            this.allConfigurations = persister.Read();
            subscriptionsRequestSource.OnRequest += (sender, message) => OnNewRequest(message);
        }

        private void OnNewRequest(RequestMessage<ConfigurationSubscription> request)
        {
            var instanceFilter = request.request.instance;
            var matchingConfigurations =
                instanceFilter == ConfigurationInstanceWildcard ?
                allConfigurations : 
                allConfigurations.Where(o => o.Instance == instanceFilter);

            subscriptionsRequestSource.Send(new AnwserMessage<ConfigurationSubscriptionAnswer>()
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