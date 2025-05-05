using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tebex.Common;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// SystemHeadlessAdapter uses the built-in C# standard libraries for API adapter functions
    /// </summary>
    public class SystemHeadlessAdapter : HeadlessAdapter
    {
        public override Task Send<T>(string url, string body, HttpVerb verb, ApiSuccessCallback onSuccess, Action<HeadlessApiError> onHeadlessApiError, Action<ServerError> onServerError)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = new HttpMethod(verb.ToString()),
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
            };

            try
            {
                return client.SendAsync(request).ContinueWith(responseTask =>
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
                            Console.WriteLine("<- " + response.StatusCode + " | " + readTask.Result);
                            onSuccess.Invoke((int)response.StatusCode, readTask.Result);
                            return Task.CompletedTask;
                        });
                    }
                    
                    // Non-success response code
                    var resultBody = response.Content.ReadAsStringAsync().Result;
                    var code = (int)response.StatusCode;
                    Console.WriteLine("<- " + resultBody);
                    
                    // 400-level responds with a nicely formatted JSON error
                    if (code >= 400 && code < 500)
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<HeadlessApiError>(resultBody);
                            if (error == null)
                            {
                                throw new JsonException("Error response body was empty, expected JSON");
                            }
                            onHeadlessApiError.Invoke(error);
                        }
                        catch (JsonException e)
                        {
                            onServerError.Invoke(new ServerError(code, e.Message + " | " + resultBody));                            
                        }
                    }
                    
                    // 500-level is something server-side and we likely won't have a JSON response
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

        public override void LogApiError(HeadlessApiError error)
        {
            Console.WriteLine("Error (" + error.ErrorCode + ") received from API: " + error.Detail);
        }

        public override void LogServerError(ServerError error)
        {
            Console.WriteLine("Unexpected server error (" + error.Code + ") from API: " + error.Body);
        }

        public override void LogDebug(string message)
        {
            Console.WriteLine($"[DEBUG] {message}");
        }
    }
}