using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// Request data for adding a package to a basket.
    /// </summary>
    public class AddPackagePayload
    {
        [JsonProperty("package_id", Required = Required.Always)]
        public int PackageId { get; private set; }

        [JsonProperty("quantity", Required = Required.Always)]
        public int Quantity { get; private set; }

        [JsonProperty("variable_data", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string>? VariableData { get; private set; }

        public AddPackagePayload(int packageId, int quantity = 1, Dictionary<string, string>? variableData = null)
        {
            PackageId = packageId;
            Quantity = quantity;
            VariableData = variableData;
        }
    }
    
    /// <summary>
    /// Request data for updating a tier.
    /// </summary>
    public class UpdateTierPayload
    {
        [JsonProperty("package_id", Required = Required.Always)]
        public int PackageId { get; private set; }

        public UpdateTierPayload(int packageId)
        {
            PackageId = packageId;
        }
    }
    
    /// <summary>
    /// Request data for creating a basket.
    /// </summary>
    public class CreateBasketPayload
    {
        [JsonProperty("email", Required = Required.Always)] public string Email { get; private set; } = string.Empty;

        [JsonProperty("username", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string OptionalUsername { get; private set; } = string.Empty; // some UUID, player ID, or otherwise unique identifier for the user

        /// <summary>
        /// Where the user is sent if cancelling the transaction
        /// </summary>
        [JsonProperty("cancel_url", Required = Required.Always)]
        public string CancelUrl { get; private set; } = string.Empty;
        
        /// <summary>
        /// Where the user is sent after completing the transaction
        /// </summary>
        [JsonProperty("complete_url", Required = Required.Always)]
        public string CompleteUrl { get; private set; } = string.Empty;
        
        [JsonProperty("custom", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Custom { get; private set; } = new Dictionary<string, string>(); // see API reference for additional fields

        /// <summary>
        /// (optional) True to automatically redirect to the relevant completion urls.
        /// </summary>
        [JsonProperty("complete_auto_redirect", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool CompleteAutoRedirect { get; private set; }

        public CreateBasketPayload(string email, string optionalUsername, string cancelUrl, string completeUrl,
            Dictionary<string, string>? custom = null, bool completeAutoRedirect = true)
        {
            Email = email;
            OptionalUsername = optionalUsername;
            CancelUrl = cancelUrl;
            CompleteUrl = completeUrl;

            if (custom == null)
            {
                custom = new Dictionary<string, string>();
            }

            Custom = custom;
            CompleteAutoRedirect = completeAutoRedirect;
        }
    }
    
    /// <summary>
    /// Request data for removing a package from a basket.
    /// </summary>
    public class PackageRemovePayload
    {
        [JsonProperty("package_id")] public int PackageId { get; private set; }

        public PackageRemovePayload(int packageId)
        {
            PackageId = packageId;
        }
    }
    
    /// <summary>
    /// Request data for setting a package's quantity.
    /// </summary>
    public class PackageQuantityPayload
    {
        [JsonProperty("quantity")]
        public int Quantity { get; private set; }

        public PackageQuantityPayload(int quantity)
        {
            Quantity = quantity;
        }
    }
}