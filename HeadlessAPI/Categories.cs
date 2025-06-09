using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// Categories are used to divide packages into sections within the store.
    /// </summary>
    public class Category
    {
        [JsonProperty("id")]
        public int Id { get; private set; } = -1;

        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty("slug")]
        public string Slug { get; private set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; private set; } = string.Empty;

        [JsonProperty("parent")]
        public PackageCategory Parent { get; private set; } = new PackageCategory();

        [JsonProperty("order")]
        public int Order { get; private set; } = -1;

        [JsonProperty("display_type")]
        public string DisplayType { get; private set; } = string.Empty;

        [JsonProperty("tiered")]
        public bool Tiered { get; private set; } = false;
        
        [JsonProperty("packages")]
        public List<Package> Packages { get; private set; } = new List<Package>();
        
        [JsonProperty("active_tier", NullValueHandling = NullValueHandling.Ignore)] 
        public Tier? ActiveTier { get; private set; }
    }

    /// <summary>
    /// A data container class for a single category.
    /// </summary>
    public class WrappedCategory
    {
        [JsonProperty("data")]
        public Category Data { get; private set; } = new Category();
    }
    
    /// <summary>
    /// A data container class for a list of categories.
    /// </summary>
    public class WrappedCategories
    {
        [JsonProperty("data")]
        public List<Category> Data { get; private set; } = new List<Category>();
    }
}