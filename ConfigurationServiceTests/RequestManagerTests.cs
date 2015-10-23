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
                new RequestManager<ConfigurationSubscription, ConfigurationUpdate>(w, RequestType.REGISTER);
            totest.OnRequest += (sender, message) => requestReceived++;
            //Test when we receive shitty data
            w.SimulateReceived(badMessage);

            Assert.AreEqual(0, requestReceived);
        }

        [Test]
        public void GoodRequestWithBadTypeIsIgnored()
        {
            var expectedRequestType = RequestType.REGISTER;
            string inputType = "SomethingElse!";
            TestWith(expectedRequestType, inputType);
        }

        [Test]
        public void GoodRequestIsForwarded()
        {
            var expectedRequestType = RequestType.REGISTER;
            string inputType = expectedRequestType.ToString();
            TestWith(expectedRequestType, inputType);
        }

        private void TestWith(RequestType expectedRequestType, string inputType)
        {
            RequestMessage<ConfigurationSubscription>? requestReceived = null;

            FakeWebsocket w = new FakeWebsocket();
            IRequestManager<ConfigurationSubscription, ConfigurationUpdate> totest =
                new RequestManager<ConfigurationSubscription, ConfigurationUpdate>(w, expectedRequestType);
            totest.OnRequest += (sender, subscriptionRequest) => requestReceived = subscriptionRequest;
            //Test when we receive good data


            var synchRequest = new ConfigurationSubscription();
            synchRequest.type = inputType;
            synchRequest.instance = "SplitonsPersistence";
            var request = new RequestMessage<ConfigurationSubscription>();
            request.id = 12;
            request.request = synchRequest;
            string serializeRequest = JsonConvert.SerializeObject(request);
            Write("Receiving " + serializeRequest);
            w.SimulateReceived(serializeRequest);

            if (expectedRequestType.ToString().Equals(inputType, StringComparison.OrdinalIgnoreCase))
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
