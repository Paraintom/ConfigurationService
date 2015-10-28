using System.Collections.Generic;

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
        string type { get; }
    }

    public struct ConfigurationSubscription : ITypedRequest
    {
        public string type { get; set; }
        public string instance { get; set; }
    }

    public struct ConfigurationSubscriptionAnswer : ITypedRequest
    {
        public string type { get { return "ConfigurationSubscriptionAnswer"; } }
        public List<Configuration> result { get; set; }
    }

    public struct ConfigurationUpdate : ITypedRequest
    {
        public string type { get; set; }
        public Configuration update { get; set; }
    }
    public struct ConfigurationUpdateAnswer : ITypedRequest
    {
        public string type { get { return "ConfigurationUpdateAnswer"; } }
        public bool result { get; set; }
    }
}