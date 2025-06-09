using Newtonsoft.Json;

namespace Tebex.HeadlessAPI
{
    public class Tier
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("created_at")] public string CreatedAt { get; private set; } = string.Empty;
        
        [JsonProperty("username_id")]
        public long UsernameId { get; private set; } = -1L;
        
        [JsonProperty("package")]
        public Package Package { get; private set; } = new Package();
        
        [JsonProperty("active")]
        public bool Active { get; private set; } = false;
        
        [JsonProperty("recurring_payment_reference")]
        public string RecurringPaymentReference { get; private set; } = string.Empty;
        
        [JsonProperty("next_payment_date")]
        public string NextPaymentDate { get; private set; } = string.Empty;

        [JsonProperty("status")]
        public TierStatus Status { get; private set; } = new TierStatus();
        
        [JsonProperty("pending_downgrade_package", NullValueHandling = NullValueHandling.Ignore)]
        public Package? PendingDowngradePackage { get; private set; }        
    }

    public class TierStatus
    {
        [JsonProperty("id")]
        public int Id { get; private set; } = -1;
        
        [JsonProperty("status")]
        public string Status { get; private set; } = string.Empty;
    }

    public class TierUpgradeResponse
    {
        [JsonProperty("success")]
        public bool Success { get; private set; } = false;
        
        [JsonProperty("message")]
        public string Message { get; private set; } = string.Empty;
    }
}