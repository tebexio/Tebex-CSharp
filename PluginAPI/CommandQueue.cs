using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    /// <summary>
    /// Metadata received from /queue
    /// </summary>
    public class CommandQueueMeta
    {
        [JsonProperty("execute_offline")] public bool ExecuteOffline { get; private set; }

        [JsonProperty("next_check")] public int NextCheck { get; private set; }

        [JsonProperty("more")] public bool More { get; private set; }
    }

    /// <summary>
    /// A due player is one returned by /queue to indicate we have some commands to run.
    /// </summary>
    public class DuePlayer
    {
        [JsonProperty("id")] public int Id { get; private set; }

        [JsonProperty("name")] public string Name { get; private set; } = string.Empty;

        [JsonProperty("uuid")] public string Uuid { get; private set; } = string.Empty;
    }
    
    public class OnlineCommandsPlayer
    {
        [JsonProperty("id")] public string Id { get; private set; } = string.Empty;
        [JsonProperty("username")] public string Username { get; private set; } = string.Empty;
        [JsonProperty("meta")] public OnlineCommandPlayerMeta Meta { get; private set; } = new OnlineCommandPlayerMeta();
    }

    public class OnlineCommandPlayerMeta
    {
        [JsonProperty("avatar")] public string Avatar { get; private set; } = string.Empty;
        [JsonProperty("avatarfull")] public string AvatarFull { get; private set; } = string.Empty;
        [JsonProperty("steamID")] public string SteamId { get; private set; } = string.Empty;
    }

    public class CommandConditions
    {
        [JsonProperty("delay")] public int Delay { get; private set;  }
        [JsonProperty("slots")] public int Slots { get; private set; }
    }

    public class OfflineCommandsMeta
    {
        [JsonProperty("limited")] public bool Limited { get; private set; }
    }

    public class Command
    {
        [JsonProperty("id")] public int Id { get; private set; }
        [JsonProperty("command")] public string CommandToRun { get; private set; } = string.Empty;
        [JsonProperty("payment")] public long Payment { get; private set; }
        [JsonProperty("package", NullValueHandling=NullValueHandling.Ignore)] public long PackageRef { get; private set; }
        [JsonProperty("conditions")] public CommandConditions Conditions { get; private set; } = new CommandConditions();
        [JsonProperty("player")] public PlayerInfo Player { get; private set; } = new PlayerInfo();
    }
    
    /// <summary>
    /// A player's information returned by the /user endpoint
    /// </summary>
    public class PlayerInfo
    {
        [JsonProperty("id")] public string Id { get; private set; } = string.Empty;
        
        [JsonProperty("name")] public string Username { get; private set; } = string.Empty;

        [JsonProperty("meta")] public OnlineCommandPlayerMeta Meta { get; private set; } = new OnlineCommandPlayerMeta();

        // Only populated by offline commands
        [JsonProperty("uuid")]
        public string Uuid { get; private set; } = string.Empty;
            
        [JsonProperty("plugin_username_id")] public int PluginUsernameId { get; private set; }
    }
}