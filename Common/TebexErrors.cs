using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.Common
{
    /// <summary>
    /// Represents an error returned by the Headless API.
    /// </summary>
    public class HeadlessApiError
    {
        [JsonProperty("status")]
        public string Status { get; private set; } = string.Empty;
        
        [JsonProperty("type")]
        public string Type { get; private set; } = string.Empty;
        
        [JsonProperty("title")]
        public string Title { get; private set; } = string.Empty;
        
        [JsonProperty("detail")]
        public string Detail { get; private set; } = string.Empty;

        [JsonProperty("error_code")] 
        public string ErrorCode { get; private set; } = string.Empty;
        
        [JsonProperty("field_details")]
        public List<string> FieldDetails { get; private set; } = new List<string>();
        
        [JsonProperty("meta")]
        public List<string> Meta { get; private set; } = new List<string>();
        
        public Exception AsException() {
            return new Exception(JsonConvert.SerializeObject(this));
        }
    }

    /// <summary>
    /// Represents a JSON error returned by the Plugin API.
    /// </summary>
    public class PluginApiError
    {
        [JsonProperty("error_code")] public int ErrorCode { get; private set; }
        [JsonProperty("error_message")] public string ErrorMessage { get; private set; } = string.Empty;
    }
    
    /// <summary>
    /// Represents a non-JSON error response code and body, typically error 500 or something else unexpected.
    /// </summary>
    public class ServerError
    {
        /// <summary>
        /// The Http response code
        /// </summary>
        public int Code { get; private set; }
        
        /// <summary>
        /// The Http response body
        /// </summary>
        public string Body { get; private set; }

        public ServerError(int code, string body)
        {
            Code = code;
            Body = body;
        }

        public Exception AsException()
        {
            return new Exception("Unexpected server error (" + Code + "): " + Body);
        }
    }
}