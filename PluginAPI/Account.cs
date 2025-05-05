using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    /// <summary>
    /// Information about the webstore account.
    /// </summary>
    public class Account
    {
        [JsonProperty("id")] public int Id { get; private set; }
        [JsonProperty("domain")] public string Domain { get; private set; } = string.Empty;
        [JsonProperty("name")] public string Name { get; private set; } = string.Empty;
        [JsonProperty("currency")] public Currency Currency { get; private set; } = new Currency();
        [JsonProperty("online_mode")] public bool OnlineMode { get; private set; }
        [JsonProperty("game_type")] public string GameType { get; private set; } = string.Empty;
        [JsonProperty("log_events")] public bool LogEvents { get; private set; }
    }

    /// <summary>
    /// The webstore currency
    /// </summary>
    public class Currency
    {
        [JsonProperty("iso_4217")] public string Iso4217 { get; private set; } = string.Empty;
        [JsonProperty("symbol")] public string Symbol { get; private set; } = string.Empty;
    }

    /// <summary>
    /// Information about the store's game server
    /// </summary>
    public class Server
    {
        [JsonProperty("id")] public int Id { get; private set; }
        [JsonProperty("name")] public string Name { get; private set; } = string.Empty;
    }

    /// <summary>
    /// Summary store information object
    /// </summary>
    public class Store
    {
        [JsonProperty("account")] public Account Account { get; private set; } = new Account();
        [JsonProperty("server")] public Server Server { get; private set; } = new Server();
    }
}