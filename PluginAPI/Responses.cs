using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    public class CheckoutResponse
    {
        [JsonProperty("url")] public string Url { get; private set; } = string.Empty;
        [JsonProperty("expires")] public string Expires { get; private set; } = string.Empty;
    }
    
    /// <summary>
    /// Response from /queue
    /// </summary>
    public class CommandQueueResponse
    {
        [JsonProperty("meta")] public CommandQueueMeta Meta { get; private set; } = new CommandQueueMeta();
        [JsonProperty("players")] public List<DuePlayer> Players { get; private set; } = new List<DuePlayer>();
    }
    
    /// <summary>
    /// Response received from /queue/online-commands
    /// </summary>
    public class OnlineCommandsResponse
    {
        [JsonProperty("player")] public OnlineCommandsPlayer Player { get; private set; } = new OnlineCommandsPlayer();
        [JsonProperty("commands")] public List<Command> Commands { get; private set; } = new List<Command>();
    }
    
    public class OfflineCommandsResponse
    {
        [JsonProperty("meta")] public OfflineCommandsMeta Meta { get; private set;  } = new OfflineCommandsMeta();
        [JsonProperty("commands")] public List<Command> Commands { get; private set;  } = new List<Command>();
    }

    /// <summary>
    /// Root object returned by the /user endpoint, containing PlayerInfo
    /// </summary>
    public class UserLookupResponse
    {
        [JsonProperty("player")] public PlayerInfo Player { get; private set; } = new PlayerInfo();

        [JsonProperty("banCount")] public int BanCount { get; private set; }

        [JsonProperty("chargebackRate")] public int ChargebackRate { get; private set; }

        [JsonProperty("payments")] public List<PlayerPaymentInfo> Payments { get; private set; } = new List<PlayerPaymentInfo>();

        [JsonProperty("purchaseTotals")] public object[] PurchaseTotals { get; private set; } = Array.Empty<object>();
    }
    
    public class ActivePackage
    {
        [JsonProperty("txn_id")]
        public string TxnId { get; private set; } = string.Empty;
        [JsonProperty("date")]
        public DateTime Date { get; private set; }
        [JsonProperty("quantity")]
        public int Quantity { get; private set; }
        [JsonProperty("package")]
        public PackageInfo Package { get; private set; } = new PackageInfo();
    }
}