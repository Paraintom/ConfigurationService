using System.Collections.Generic;
using System.Linq;
using ConfigurationService.Persistence;
using ConfigurationService.WebSocketInterface;

namespace ConfigurationService
{
    public class ConfigurationRequestHandler
    {
        private IStatePersister persister;
        private object internalLock = new object();
        private List<Configuration> allConfigurations;
        public const string ConfigurationInstanceWildcard = "*";

        public ConfigurationRequestHandler(
            IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer> subscriptionsRequestSource,
            IRequestManager<ConfigurationUpdate, ConfigurationUpdateAnswer> updateRequestManager, 
            IStatePersister persister)
        {
            this.persister = persister;
            this.allConfigurations = persister.Read();
            subscriptionsRequestSource.OnRequest += (sender, message) => OnNewSubscription(subscriptionsRequestSource,message);
            updateRequestManager.OnRequest += (sender, message) => OnNewUpdate(updateRequestManager,message);
        }

        private void OnNewUpdate(
            IRequestManager<ConfigurationUpdate, ConfigurationUpdateAnswer> sender, 
            RequestMessage<ConfigurationUpdate> request)
        {
            var newConfiguration = request.request.update;
            //Not the most efficient code, but premature optimisation is root of evil.
            lock (internalLock)
            {
                var existing = allConfigurations.FirstOrDefault(o =>
                    o.Instance == newConfiguration.Instance &&
                    o.Key == newConfiguration.Key
                );
                if (existing != null)
                {
                    existing.Value = newConfiguration.Value;
                }
                else
                {
                    allConfigurations.Add(newConfiguration);
                }
            }
            this.persister.Persist(allConfigurations);
            //Todo here broadcast the update?
            sender.Send(new AnwserMessage<ConfigurationUpdateAnswer>()
            {
                id = request.id,
                answer = new ConfigurationUpdateAnswer()
                {
                    result = true
                }
            });
        }

        private void OnNewSubscription(
            IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer> sender, 
            RequestMessage<ConfigurationSubscription> request)
        {

            IEnumerable<Configuration> result;
            //Not the most efficient code, but premature optimisation is root of evil.
            lock (internalLock)
            {
                var instanceFilter = request.request.instance;
                var matchingConfigurations = instanceFilter == ConfigurationInstanceWildcard
                    ? allConfigurations
                    : allConfigurations.Where(o => o.Instance == instanceFilter);
                result = matchingConfigurations;
            }

            sender.Send(new AnwserMessage<ConfigurationSubscriptionAnswer>()
            {
                id = request.id,
                answer = new ConfigurationSubscriptionAnswer()
                {
                    result = result.ToList()
                }
            });
        }
    }
}