using System;
using Newtonsoft.Json;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// The main Tebex project / store information.
    /// </summary>
    public class Webstore
    {
        [JsonProperty(PropertyName = "id")]
        public uint ID { get; private set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "webstore_url")]
        public string WebstoreUrl { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "lang")]
        public string Lang { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "logo_url")]
        public string Logo { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "platform_type")]
        public string PlatformType { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "platform_type_id")]
        public string PlatformTypeId { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; private set; } = DateTime.MinValue;
    }
}