using System.Collections.Generic;
using System.Linq;
using ConfigurationService;
using ConfigurationService.Persistence;
using ConfigurationService.WebSocketInterface;
using Moq;
using NUnit.Framework;

namespace ConfigurationServiceTests
{
    class ConfigurationRequestHandlerTests : BaseTest
    {
        [Test]
        public void BasicTest()
        {
            string goodInstance = "goodInstance";
            var statePersister = new Mock<IStatePersister>();
            // Creating fake data for the statePersister...
            var configurationsPersisted = new List<Configuration>()
            {
                GetConfig(1),
                GetConfig(2),
                GetConfig(3)
            };
            Configuration firstGoodConfiguration = GetConfig(1);
            Configuration secondGoodConfiguration = GetConfig(6);
            firstGoodConfiguration.Instance = goodInstance;
            secondGoodConfiguration.Instance = goodInstance;
            configurationsPersisted.Add(firstGoodConfiguration);
            configurationsPersisted.Add(secondGoodConfiguration);

            statePersister.Setup(o => o.Read()).Returns(configurationsPersisted);

            //Creating a fake requestmanager.
            var subscriptionsRequest = new Mock<IRequestManager<ConfigurationSubscription, ConfigurationSubscriptionAnswer>>();
            var updateRequest = new Mock<IRequestManager<ConfigurationUpdate, ConfigurationUpdateAnswer>>();
            var allAnswerSent = new List<AnwserMessage<ConfigurationSubscriptionAnswer>>();
            subscriptionsRequest.Setup(o => o.Send(It.IsAny<AnwserMessage<ConfigurationSubscriptionAnswer>>())).
                Callback<AnwserMessage<ConfigurationSubscriptionAnswer>>(allAnswerSent.Add);

            var toTest = new ConfigurationRequestHandler(subscriptionsRequest.Object, updateRequest.Object, statePersister.Object);
            
            //Sending a request to the manager
            var requestId = 1021;
            var request = GetRequestMessage(requestId, goodInstance);
            subscriptionsRequest.Raise(mock => mock.OnRequest += null, null, request);

            //We check that we received what we should, one message with two elements
            Assert.AreEqual(1, allAnswerSent.Count);
            Assert.AreEqual(requestId, allAnswerSent.First().id);
            var sentConfigurations = allAnswerSent.First().answer.result;
            Assert.AreEqual(2, sentConfigurations.Count);
            Assert.AreEqual(firstGoodConfiguration, sentConfigurations.First());
            Assert.AreEqual(secondGoodConfiguration, sentConfigurations.Last());

            //test wildcard :
            var requestAll = GetRequestMessage(requestId+1, ConfigurationRequestHandler.ConfigurationInstanceWildcard);
            subscriptionsRequest.Raise(mock => mock.OnRequest += null, null, requestAll);
            Assert.AreEqual(configurationsPersisted.Count, allAnswerSent.Last().answer.result.Count);
        }

        private static RequestMessage<ConfigurationSubscription> GetRequestMessage(int requestId, string goodInstance)
        {
            var request = new RequestMessage<ConfigurationSubscription>()
            {
                id = requestId,
                request = new ConfigurationSubscription() {instance = goodInstance}
            };
            return request;
        }

        private static Configuration GetConfig(int configNumber)
        {
            return new Configuration()
            {
                Instance = string.Format("Instance{0}", configNumber),
                Key = string.Format("Key{0}", configNumber),
                Value = string.Format("Value{0}", configNumber)
            };
        }
    }
}
