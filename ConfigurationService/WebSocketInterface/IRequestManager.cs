using System;

namespace ConfigurationService.WebSocketInterface
{
    public interface IRequestManager<I, O>
        where I : ITypedRequest
        where O : ITypedRequest
    {
        event EventHandler<RequestMessage<I>> OnRequest;
        string ExpectedRequestType { get; }
        void Send(AnwserMessage<O> answer);
    }
}
