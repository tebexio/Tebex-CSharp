using System;
using System.Threading.Tasks;
using Tebex.Common;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// HeadlessAdapter defines the operations needed to interface into a given platform. For example, some platforms
    /// may not allow us to use the standard library to send HTTP requests and include their own methods for doing so.
    ///
    /// As needed per platform, we define a HeadlessAdapter that calls the necessary underlying functionality.
    /// </summary>
    public abstract class HeadlessAdapter
    {
        internal TebexHeadlessApi _apiInstance;
        internal void SetApiInstance(TebexHeadlessApi apiInstance)
        {
            _apiInstance = apiInstance;
        }
        
        /// <summary>
        /// Sends an HTTP request to the specified URL with the given parameters and handles the response using the provided callbacks.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the response that is expected upon a successful request.</typeparam>
        /// <param name="url">The endpoint URL to which the request will be sent.</param>
        /// <param name="body">The body of the HTTP request to be sent.</param>
        /// <param name="verb">The HTTP method to be used for the request (e.g., GET, POST, PUT, DELETE, PATCH).</param>
        /// <param name="onSuccess">Callback executed when the request is successful. Receives the response code and body as parameters.</param>
        /// <param name="onHeadlessApiError">Callback executed when there is a client-side error specific to the Headless API.</param>
        /// <param name="onServerError">Callback executed when the server responds with an error such as 500 Internal Server Error.</param>
        /// <param name="authenticated">True if we should use an authenticated call (HTTP Basic, will require public key and private key to be set</param>
        /// <returns>An asynchronous task for the process of sending the HTTP request and handling the response.</returns>
        public abstract Task Send<TReturnType>(string url, string body, HttpVerb verb, ApiSuccessCallback onSuccess,
            Action<HeadlessApiError> onHeadlessApiError, Action<ServerError> onServerError, bool authenticated = false);

        public abstract void LogApiError(HeadlessApiError error);

        public abstract void LogServerError(ServerError error);
        
        public abstract void LogDebug(string message);
    }
}