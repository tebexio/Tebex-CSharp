using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tebex.Common;
using Tebex.PluginAPI;

namespace Tebex.Plugin
{
    /// <summary>
    /// Represents a PluginAdapter that uses the C# standard library as its implementing interfaces.
    /// </summary>
    public class StandardPluginAdapter : PluginAdapter
    {
        private static readonly HttpClient Client = new HttpClient();
        
        public override Task Send<T>(string key, string url, string body, HttpVerb verb, ApiSuccessCallback onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = new HttpMethod(verb.ToString()),
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Add("X-Tebex-Secret", key);

            try
            {
                LogDebug("-> " + verb + " " + url + " | " + body);
                return Client.SendAsync(request).ContinueWith(responseTask =>
                {
                    if (responseTask.IsFaulted) // Some exception occurred if this is set
                    {
                        return Task.FromException(responseTask.Exception);
                    }
                    
                    // No exception during the request, continue with processing
                    var response = responseTask.Result;
                    if (response.IsSuccessStatusCode) // 200 OK or similar
                    {
                        return response.Content.ReadAsStringAsync().ContinueWith(readTask =>
                        {
                            if (readTask.IsFaulted) // Exception occurred during reading the body
                            {
                                return Task.FromException(readTask.Exception);
                            }
                            // No exception, request completed successfully
                            LogDebug("<- " + response.StatusCode + " | " + readTask.Result);
                            onSuccess.Invoke((int)response.StatusCode, readTask.Result);
                            return Task.CompletedTask;
                        });
                    }
                    
                    // Non-success response code
                    var resultBody = response.Content.ReadAsStringAsync().Result;
                    var code = (int)response.StatusCode;
                    LogDebug("<- " + code + " | " + resultBody);
                    
                    // 400-level responds with a nicely formatted JSON error
                    if (code >= 400 && code < 500)
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<PluginApiError>(resultBody);
                            if (error == null)
                            {
                                throw new JsonException("Error response body was empty, expected JSON");
                            }
                            onPluginApiError.Invoke(error);
                        }
                        catch (JsonException e)
                        {
                            onServerError.Invoke(new ServerError(code, e.Message + " | " + resultBody));                            
                        }
                    }
                    
                    // 500-level is something server-side
                    else if (code >= 500 && code < 600)
                    {
                        onServerError.Invoke(new ServerError(code, resultBody));
                    }

                    else // Any unexpected status code
                    {
                        onServerError.Invoke(new ServerError(code, resultBody));
                    }
                    
                    return Task.CompletedTask;
                });
            } 
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        public override bool PlayerHasInventorySlotsAvailable(DuePlayer player, int slots)
        {
            throw new NotImplementedException();
        }

        public override bool ExecuteCommand(Command command)
        {
            throw new NotImplementedException();
        }

        public override bool IsPlayerOnline(DuePlayer player)
        {
            throw new NotImplementedException();
        }

        public override bool IsDebugModeEnabled()
        {
            return true;
        }

        public override void TellPlayer(string userId, string message)
        {
            throw new NotImplementedException();
        }
    }
}