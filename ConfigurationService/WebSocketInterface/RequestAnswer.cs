namespace ConfigurationService.WebSocketInterface
{
    // ReSharper disable InconsistentNaming because ofserialisation Json.
    public struct RequestMessage<T>
    {
        public int id { get; set; }
        public T request { get; set; }
    }
    public struct AnwserMessage<T>
    {
        public int id { get; set; }
        public T answer { get; set; }
    }
    // ReSharper restore InconsistentNaming
}
