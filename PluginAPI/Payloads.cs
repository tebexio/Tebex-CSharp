using System;
using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    /// <summary>
    /// Represents the payload for deleting commands on the game server.
    /// </summary>
    public class DeleteCommandsPayload
    {
        /// <summary>
        /// An array of one or more command ids to delete
        /// </summary>
        [JsonProperty("ids")]
        public int[] Ids { get; set; } = Array.Empty<int>();
    }
    
    /// <summary>
    /// Represents the payload for creating a checkout link for a player.
    /// </summary>
    public class CreateCheckoutPayload
    {
        [JsonProperty("package_id")] public int PackageId { get; set; }

        [JsonProperty("username")] public string Username { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Represents the payload for creating a gift card on the store.
    /// </summary>
    public class CreateGiftCardPayload
    {
        [JsonProperty("expires_at")] public DateTime ExpiresAt { get; set; }
        [JsonProperty("note")] public string Note { get; set; } = string.Empty;
        [JsonProperty("amount")] public double Amount { get; set; }
    }
    
    /// <summary>
    /// Represents the payload for adding an amount to a giftcard.
    /// </summary>
    public class TopUpGiftCardPayload
    {
        [JsonProperty("amount")] public string Amount { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Represents the payload for banning a player from the store.
    /// </summary>
    public class CreateBanPayload
    {
        [JsonProperty("reason")] public string Reason { get; set; } = string.Empty;
        [JsonProperty("ip")] public string IP { get; set; } = string.Empty;

        /// <summary>
        /// Username or uuid of the player to ban
        /// </summary>
        [JsonProperty("user")]
        public string User { get; set; } = string.Empty;
    }
}