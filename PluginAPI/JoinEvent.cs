using System;
using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    public class JoinEvent
    {
        [JsonProperty("username_id")] // steam64ID is appropriate here
        public string UsernameId { get; private set; }
        [JsonProperty("event_type")]
        public string EventType { get; private set; }
        [JsonProperty("event_date")]
        public DateTime EventDate { get; private set; }
        [JsonProperty("ip")]
        public string IpAddress { get; private set; }

        public JoinEvent(string usernameId, string eventType, string ipAddress)
        {
            UsernameId = usernameId;
            EventType = eventType;
            EventDate = DateTime.UtcNow;
            IpAddress = ipAddress;
        }
    }   
}