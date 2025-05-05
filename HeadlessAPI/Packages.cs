using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// A Package is a purchasable item in the store.
    /// </summary>
    public class Package
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; private set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; private set; } = string.Empty;

        [JsonProperty("category")]
        public PackageCategory Category { get; private set; } = new PackageCategory();

        [JsonProperty("base_price")] public float BasePrice { get; private set; }

        [JsonProperty("sales_tax")]
        public float SalesTax { get; private set; }

        [JsonProperty("total_price")]
        public float TotalPrice { get; private set; }

        [JsonProperty("currency")]
        public string Currency { get; private set; } = string.Empty;

        [JsonProperty("discount")]
        public float Discount { get; private set; }

        [JsonProperty("disable_quantity")]
        public bool DisableQuantity { get; private set; }

        [JsonProperty("disable_gifting")]
        public bool DisableGifting { get; private set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; private set; } = string.Empty;

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; private set; } = string.Empty;

        [JsonProperty("order")]
        public int Order { get; private set; } = -1;

        [JsonProperty("variables")] 
        public List<PackageVariable> Variables = new List<PackageVariable>();
    }
    
    /// <summary>
    /// A data container class for a single package.
    /// </summary>
    public class WrappedPackage
    {
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public Package Data { get; private set; } = new Package();
    }
    
    /// <summary>
    /// A reference to a package's category.
    /// </summary>
    public class PackageCategory
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; } = string.Empty;
    }
    
    /// <summary>
    /// A data container class for a list of packages.
    /// </summary>
    public class WrappedPackages
    {
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public List<Package> Data { get; private set; } = new List<Package>();
    }

    /// <summary>
    /// The variable_data variables associated with a package.
    /// </summary>
    public class PackageVariable
    {
        [JsonProperty("id")]
        public int Id { get; private set; } = -1;
        
        [JsonProperty("identifier")]
        public string Identifier { get; private set; } = string.Empty;
        
        [JsonProperty("description")]
        public string Description { get; private set; } = string.Empty;
        
        [JsonProperty("min_length")]
        public int MinLength { get; private set; }
        
        [JsonProperty("max_length")]
        public int MaxLength { get; private set; }
        
        [JsonProperty("type")]
        public string Type { get; private set; } = string.Empty;
        
        [JsonProperty("allow_colors")]
        public bool AllowColors { get; private set; }
        
        [JsonProperty("options", Required = Required.Default)]
        public List<PackageVariableOption> Options { get; private set; } = new List<PackageVariableOption>();
    }

    /// <summary>
    /// An option for a package's variable_data variables.
    /// </summary>
    public class PackageVariableOption
    {
        [JsonProperty("id")]
        public int Id { get; private set; } = -1;
        
        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;
        
        [JsonProperty("value")]
        public string Value { get; private set; } = string.Empty;

        [JsonProperty("price")]
        public double Price { get; private set; }
        
        [JsonProperty("percentage")]
        public double Percentage { get; private set; }
    }
}