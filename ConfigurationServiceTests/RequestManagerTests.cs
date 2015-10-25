using System;
using ConfigurationService.WebSocketInterface;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ConfigurationServiceTests
{
    [TestFixture]
    public class RequestManagerTests : BaseTest
    {
        [Test]
        public void BadMessageIsIgnored()
        {
            int requestReceived = 0;
            string badMessage = "ha ha, not a Json request!";
            FakeWebsocket w = new FakeWebsocket();
            IRequestManager<ConfigurationSubscription, ConfigurationUpdate> totest =
                new RequestManager<ConfigurationSubscription, ConfigurationUpdate>(w);
            totest.OnRequest += (sender, message) => requestReceived++;
            //Test when we receive shitty data
            w.SimulateReceived(badMessage);

            Assert.AreEqual(0, requestReceived);
        }

        [Test]
        public void GoodRequestWithBadTypeIsIgnored()
        {
            TestWith(false);
        }

        [Test]
        public void GoodRequestIsForwarded()
        {
            TestWith(true);
        }

        private void TestWith(bool goodInputType)
        {
            RequestMessage<ConfigurationSubscription>? requestReceived = null;

            FakeWebsocket w = new FakeWebsocket();
            IRequestManager<ConfigurationSubscription, ConfigurationUpdate> totest =
                new RequestManager<ConfigurationSubscription, ConfigurationUpdate>(w);
            var expectedRequestType = totest.ExpectedRequestType;
            totest.OnRequest += (sender, subscriptionRequest) => requestReceived = subscriptionRequest;
            //Test when we receive good data


            var synchRequest = new ConfigurationSubscription();
            synchRequest.type = goodInputType ? expectedRequestType : "SomethingElse!";
            synchRequest.instance = "SplitonsPersistence";
            var request = new RequestMessage<ConfigurationSubscription>();
            request.id = 12;
            request.request = synchRequest;
            string serializeRequest = JsonConvert.SerializeObject(request);
            Write("Receiving " + serializeRequest);
            w.SimulateReceived(serializeRequest);

            if (goodInputType)
            {
                Assert.IsNotNull(requestReceived.HasValue);
                Assert.AreEqual(request.id, requestReceived.Value.id);
                Assert.AreEqual(request.request.type, requestReceived.Value.request.type);
                Assert.AreEqual(request.request.instance, requestReceived.Value.request.instance);
            }
            else
            {
                Assert.IsFalse(requestReceived.HasValue);
            }
        }
    }
}
