namespace ConfigurationService.WebSocketInterface
{
    public enum RequestType
    {
        REGISTER,
        UPDATE
    }

    public interface ITypedRequest
    {
        /// <summary>
        /// For allowed values see RequestType.
        /// </summary>
        string type { get; set; }
    }

    public struct ConfigurationSubscription : ITypedRequest
    {
        public string type { get; set; }
        public string instance { get; set; }
    }

    public struct ConfigurationUpdate : ITypedRequest
    {
        public string type { get; set; }
        public Configuration update { get; set; }
    }
}