using System;

namespace ConfigurationService.WebSocketInterface
{
    interface IRequestFlickerConnection : IWebsocket, IDisposable
    {
        bool IsUp { get; }
        event EventHandler OnDisconnected;
        void Start();
    }
}
