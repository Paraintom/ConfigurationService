using System;

namespace ConfigurationService.WebSocketInterface
{
    public interface IWebsocket
    {
        event EventHandler<string> OnMessage;
        void Send(string message);
    }
}
