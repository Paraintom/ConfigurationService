using System;
using ConfigurationService.Events;
using Newtonsoft.Json;
using NLog;

namespace ConfigurationService.WebSocketInterface
{

    public class JsonRequestManager<I, O> : IRequestManager<I, O> 
        where I : ITypedRequest
        where O : ITypedRequest
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private IWebsocket Connection;
        public string ExpectedRequestType { get; private set; }
        public JsonRequestManager(IWebsocket connection)
        {
            this.Connection = connection;
            this.Connection.OnMessage += OnConnectionMessage;
            this.ExpectedRequestType = typeof(I).Name;
        }

        public event EventHandler<RequestMessage<I>> OnRequest;

        private void OnConnectionMessage(object sender, string s)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<RequestMessage<I>>(s);

                //dynamic request = JObject.Parse(s);
                string requestTypeString = request.request.type;
                if (!string.IsNullOrEmpty(requestTypeString) && ExpectedRequestType.Equals(requestTypeString, StringComparison.OrdinalIgnoreCase))
                {
                    logger.Info("Request {1} received of type {0}", request.request.type, request.id);

                    this.OnRequest.RaiseEvent(this, request);
                }
                else
                {
                    //error case : projectId is null or empty
                    logger.Debug("Ignoring request of type {0}", request.request.type);
                }
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("Bad incoming websocket message received : {0}", s), ex);
            }
        }

        public void Send(AnwserMessage<O> answer)
        {
            string msg = JsonConvert.SerializeObject(answer);

            logger.Info("Sending answer for request {0} : {1}", answer.id, msg);
            Connection.Send(msg);
        }
    }
}
