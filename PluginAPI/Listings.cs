using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    /// <summary>
    /// Represents a category within the webstore.
    /// </summary>
    public class Category
    {
        [JsonProperty("id")] public int Id { get; private set; }
        [JsonProperty("order")] public int Order { get; private set; }
        [JsonProperty("name")] public string Name { get; private set; } = string.Empty;
        [JsonProperty("only_subcategories")] public bool OnlySubcategories { get; private set; }
        [JsonProperty("subcategories")] public List<Category> Subcategories { get; private set; } = new List<Category>();
        [JsonProperty("packages")] public List<Package> Packages { get; private set; } = new List<Package>();
        [JsonProperty("gui_item")] public object? GuiItem { get; private set; }
    }

    /// <summary>
    /// Represents a purchasable package on the store.
    /// </summary>
    public class Package
    {
        [JsonProperty("id")] public int Id { get; private set; }

        [JsonProperty("name")] public string Name { get; private set; } = string.Empty;

        [JsonProperty("order")] public int Order { get; private set; } = -1;

        [JsonProperty("image")] public string Image { get; private set; } = string.Empty;

        [JsonProperty("price")] public double Price { get; private set; } = -1.0d;

        [JsonProperty("sale")] public PackageSaleData? Sale { get; private set; }

        [JsonProperty("expiry_length")] public int ExpiryLength { get; private set; }

        [JsonProperty("expiry_period")] public string ExpiryPeriod { get; private set; } = string.Empty;

        [JsonProperty("type")] public string Type { get; private set; } = string.Empty;

        [JsonProperty("category")] public Category Category { get; private set; } = new Category();

        [JsonProperty("global_limit")] public int GlobalLimit { get; private set; }

        [JsonProperty("global_limit_period")] public string GlobalLimitPeriod { get; private set; } = string.Empty;

        [JsonProperty("user_limit")] public int UserLimit { get; private set; }

        [JsonProperty("user_limit_period")] public string UserLimitPeriod { get; private set; } = string.Empty;

        [JsonProperty("servers")] public List<Server>? Servers { get; private set; }

        [JsonProperty("required_packages")] public List<object>? RequiredPackages { get; private set; }

        [JsonProperty("require_any")] public bool RequireAny { get; private set; }

        [JsonProperty("create_giftcard")] public bool CreateGiftcard { get; private set; }

        [JsonProperty("show_until")] public string ShowUntil { get; private set; } = string.Empty;

        [JsonProperty("gui_item")] public string GuiItem { get; private set; } = string.Empty;

        [JsonProperty("disabled")] public bool Disabled { get; private set; }

        [JsonProperty("disable_quantity")] public bool DisableQuantity { get; private set; }

        [JsonProperty("custom_price")] public bool CustomPrice { get; private set; }

        [JsonProperty("choose_server")] public bool ChooseServer { get; private set; }

        [JsonProperty("limit_expires")] public bool LimitExpires { get; private set; }

        [JsonProperty("inherit_commands")] public bool InheritCommands { get; private set; }

        [JsonProperty("variable_giftcard")] public bool VariableGiftcard { get; private set; }

        /// <remarks>
        /// Description is not provided unless verbose=true is passed to the Packages endpoint
        /// </remarks>
        [JsonProperty("description")] public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Converts the payment type and associated data of the package into a user-friendly string representation.
        /// </summary>
        /// <returns>A user-readable string describing the payment frequency, such as "One-Time" or "Each X Periods". If the payment type is unrecognized, returns "???".</returns>
        public string GetFriendlyPayFrequency()
        {
            switch (Type)
            {
                case "single": return "One-Time";
                case "subscription": return $"Each {ExpiryLength} {ExpiryPeriod}";
                default: return "???";
            }
        }
    }

    public class PackageSaleData
    {
        [JsonProperty("active")] public bool Active { get; private set; } = false;
        [JsonProperty("discount")] public double Discount { get; private set; } = -1.0d;
    }
    
    /// <summary>
    /// Response to /listing on Plugin API
    /// </summary>
    public class ListingsResponse
    {
        [JsonProperty("categories")] public List<Category> Categories { get; private set; } = new List<Category>();
    }
}