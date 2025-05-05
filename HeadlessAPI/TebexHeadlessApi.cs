using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tebex.Common;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// Tebex Headless API allows integration of your store directly into your own frontend or in-game.
    /// </summary>
    public class TebexHeadlessApi
    {
        private static readonly string HeadlessApiBase = "https://headless.tebex.io/api/";
        private string PublicToken { get; set; } = string.Empty;
        
        private HeadlessAdapter _adapter;
        private Webstore? _store;

#pragma warning disable CS8618, CS9264
        private TebexHeadlessApi() {} // non-nullable fields are assigned in Initialize()
#pragma warning restore CS8618, CS9264

        /// <summary>
        /// Initializes a new instance of the TebexHeadlessApi class with the provided adapter and public token.
        /// </summary>
        /// <param name="adapter">Use the SystemHeadlessAdapter if System libraries will suffice.</param>
        /// <param name="publicToken">The public API token for authentication with the Tebex store.</param>
        /// <returns>A new instance of the TebexHeadlessApi class configured with the provided adapter and public token.</returns>
        public static TebexHeadlessApi Initialize(HeadlessAdapter adapter, string publicToken)
        {
            var api = new TebexHeadlessApi
            {
                _adapter = adapter
            };
            api.SetStoreToken(publicToken);
            return api;
        }
        
        /// <summary>
        /// Sets the currently configured public token used for API requests
        /// </summary>
        /// <param name="token"></param>
        public void SetStoreToken(string token)
        {
            PublicToken = token;
            GetWebstore(webstore => _store = webstore, _adapter.LogApiError, _adapter.LogServerError).Wait();
        }

        /// <summary>
        /// Sends an HTTP request to a specified endpoint using the given parameters and invokes appropriate callbacks based on the response.
        /// </summary>
        /// <param name="endpoint">The API endpoint to which the request will be sent.</param>
        /// <param name="body">The request payload in JSON format, if applicable.</param>
        /// <param name="method">The HTTP verb (GET, POST, PUT, etc.) to be used for the request.</param>
        /// <param name="onSuccess">The callback invoked on a successful response, containing the HTTP status code and response body.</param>
        /// <param name="onHeadlessApiError">The callback invoked when a headless API-specific error occurs.</param>
        /// <param name="onServerError">The callback invoked when an unknown/server error occurs.</param>
        /// <typeparam name="TReturnType">The type of the object to be deserialized from the response.</typeparam>
        /// <returns>A task representing the asynchronous operation of sending the HTTP request.</returns>
        private Task Send<TReturnType>(string endpoint, string body, HttpVerb method, ApiSuccessCallback onSuccess,
            Action<HeadlessApiError> onHeadlessApiError, Action<ServerError> onServerError)
        {
            return _adapter.Send<TReturnType>(HeadlessApiBase + endpoint, body, method, onSuccess, onHeadlessApiError,
                onServerError);
        }

        /// <summary>
        /// Retrieves the webstore associated with the provided public token.
        /// </summary>
        /// <param name="onSuccess">The action to execute when the webstore is successfully retrieved. Provides an instance of <see cref="Webstore"/>.</param>
        /// <param name="onApiError">The action to execute when an API error occurs. Provides an instance of <see cref="HeadlessApiError"/>.</param>
        /// <param name="onServerError">The action to execute when a server error occurs. Provides an instance of <see cref="ServerError"/>.</param>
        /// <returns>A task representing the asynchronous operation of retrieving the webstore.</returns>
        public Task GetWebstore(Action<Webstore> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken, onSuccess, onApiError, onServerError);
        }

        /// <summary>
        /// Retrieves a list of all packages available in the webstore.
        /// </summary>
        /// <param name="onSuccess">The action to execute on success. Provides an instance of <see cref="WrappedPackages"/>.</param>
        /// <param name="onApiError">The action to execute when an API error occurs. Provides an instance of <see cref="HeadlessApiError"/>.</param>
        /// <param name="onServerError">The action to execute when a server error occurs. Provides an instance of <see cref="ServerError"/>.</param>
        /// <returns>An asynchronous operation representing the process of retrieving all packages.</returns>
        public Task GetAllPackages(Action<WrappedPackages> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/packages", onSuccess, onApiError, onServerError);
        }

        /// <summary>
        /// Retrieves a specific package by its ID from the webstore.
        /// </summary>
        /// <param name="packageId">The unique identifier of the package to retrieve.</param>
        /// <param name="onSuccess">The action to execute on successProvides an instance of <see cref="WrappedPackage"/>.</param>
        /// <param name="onApiError">The action to execute when an API error occurs. Provides an instance of <see cref="HeadlessApiError"/>.</param>
        /// <param name="onServerError">The action to execute when a server error occurs. Provides an instance of <see cref="ServerError"/>.</param>
        /// <returns>A Task representing the asynchronous operation of retrieving the package information.</returns>
        public Task GetPackage(int packageId, Action<WrappedPackage> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/packages/" + packageId, onSuccess, onApiError, onServerError);
        }

        public Task GetPackage(int packageId, string basketIdent, Action<WrappedPackage> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/packages/" + packageId + "?basketIdent=" + basketIdent, onSuccess, onApiError, onServerError);
        }
        
        public Task GetPackage(int packageId, string ipAddress, string basketIdent, Action<WrappedPackage> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/packages/" + packageId + "?basketIdent=" + basketIdent + "&ipAddress=" + ipAddress, onSuccess, onApiError, onServerError);
        }

        public Task GetAllCategories(Action<WrappedCategories> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/categories", onSuccess, onApiError, onServerError);
        }
        
        public Task GetAllCategoriesIncludingPackages(Action<WrappedCategories> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/categories?includePackages=1", onSuccess, onApiError, onServerError);
        }
        
        public Task GetCategory(int categoryId, Action<WrappedCategory> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/categories/" + categoryId, onSuccess, onApiError, onServerError);
        }
        
        public Task GetCategoryIncludingPackages(int categoryId, Action<WrappedCategory> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/categories/" + categoryId + "?includePackages=1", onSuccess, onApiError, onServerError);
        }
        
        public Task GetBasket(string basketIdent, Action<WrappedBasket> onSuccess, Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent, onSuccess, onApiError, onServerError);
        }

        public Task CreateBasket(CreateBasketPayload basketPayload, Action<WrappedBasket> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            return PostRequestAsync("accounts/" + PublicToken + "/baskets", JsonConvert.SerializeObject(basketPayload), onSuccess, onApiError, onServerError);
        }

        public Task AddPackageToBasket(string basketIdent, AddPackagePayload payload, Action<WrappedBasket> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            return PostRequestAsync("baskets/" + basketIdent + "/packages", JsonConvert.SerializeObject(payload), onSuccess, onApiError, onServerError);            
        }

        public Task UpdatePackageQuantity(string basketIdent, int packageId, int newQuantity,
            Action<WrappedBasket> onSuccess, Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            var payload = new PackageQuantityPayload(newQuantity);
            return PutRequestAsync("baskets/" + basketIdent + "/packages/" + packageId, JsonConvert.SerializeObject(payload), onSuccess, onApiError, onServerError);
        }
        
        public Task GetBasketAuthLinks(string basketIdent, string returnUrl, Action<WrappedBasketLinks> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            return SendRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent + "/auth?returnUrl=" + returnUrl, onSuccess, onApiError, onServerError);
        }
        
        public Task RemovePackageFromBasket(string basketIdent, int packageId, Action<WrappedBasket> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            var payload = new PackageRemovePayload(packageId);
            return PostRequestAsync("baskets/" + basketIdent + "/packages/remove", JsonConvert.SerializeObject(payload), onSuccess, onApiError, onServerError);            
        }
        
        public Task ApplyCreatorCode(string basketIdent, string creatorCode, Action<EmptyResponse> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            var payload = new CreatorCode(creatorCode);
            return PostRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent + "/creator-codes", JsonConvert.SerializeObject(payload), onSuccess, onApiError, onServerError);
        }
        
        public Task RemoveCreatorCode(string basketIdent, Action<EmptyResponse> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            return PostRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent + "/creator-codes/remove", "", onSuccess, onApiError, onServerError);
        }
        
        public Task ApplyGiftCard(string basketIdent, string giftCardCode, Action<WrappedBasket> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            var giftCard = new GiftCard(giftCardCode);
            return PostRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent + "/giftcards", JsonConvert.SerializeObject(giftCard), onSuccess, onApiError, onServerError);
        }
        
        public Task RemoveGiftCard(string basketIdent, string giftCardCode, Action<EmptyResponse> onSuccess, Action<HeadlessApiError> onApiError,
            Action<ServerError> onServerError)
        {
            var giftCard = new GiftCard(giftCardCode);
            return PostRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent + "/giftcards/remove", JsonConvert.SerializeObject(giftCard), onSuccess, onApiError, onServerError);
        }
        
        public Task ApplyCoupon(string basketIdent, string couponCode, Action<EmptyResponse> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            var coupon = new BasketCoupon(couponCode);
            return PostRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent + "/coupons", JsonConvert.SerializeObject(coupon), onSuccess, onApiError, onServerError);
        }
        
        public Task RemoveCoupon(string basketIdent, string couponCode, Action<EmptyResponse> onSuccess,
            Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            var coupon = new BasketCoupon(couponCode);
            return PostRequestAsync("accounts/" + PublicToken + "/baskets/" + basketIdent + "/coupons/remove", JsonConvert.SerializeObject(coupon), onSuccess, onApiError, onServerError);
        }
        
        private Task SendRequestAsync<T>(string endpoint, Action<T> onSuccess, Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            _adapter.LogDebug("-> GET " + endpoint);
            return Send<T>(endpoint, "", HttpVerb.GET, (code, body) =>
            {
                HandleResponse(endpoint, body, onSuccess, onApiError, onServerError, code);
            }, onApiError.Invoke, onServerError.Invoke);
        }
        
        private Task PostRequestAsync<T>(string endpoint, string data, Action<T> onSuccess, Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            _adapter.LogDebug("-> POST " + endpoint + " | " + data);
            return Send<T>(endpoint, data, HttpVerb.POST, (code, body) =>
            {
                HandleResponse(endpoint, body, onSuccess, onApiError, onServerError, code);
            }, onApiError.Invoke, onServerError.Invoke);
        }
        
        private Task PutRequestAsync<TReturnType>(string endpoint, string data, Action<TReturnType> onSuccess, Action<HeadlessApiError> onApiError, Action<ServerError> onServerError)
        {
            _adapter.LogDebug("-> PUT " + endpoint + " | " + data);
            return Send<TReturnType>(endpoint, data, HttpVerb.PUT, (code, body) =>
            {
                HandleResponse(endpoint, body, onSuccess, onApiError, onServerError, code);
            }, onApiError.Invoke, onServerError.Invoke);
        }
        
        private void HandleResponse<TReturnType>(string url, string body, Action<TReturnType> onSuccess, Action<HeadlessApiError> onApiError, Action<ServerError> onServerError, int statusCode)
        {
            try
            {
                _adapter.LogDebug($"<- {statusCode} {url} | {body}");
                
                // baskets/{ident}/auth will return an array containing an empty array "[[]]" instead of an empty object, handle that here
                if (typeof(TReturnType) == typeof(WrappedBasketLinks) && body.StartsWith("["))
                {
                    var emptyBasketLinks = Activator.CreateInstance(typeof(WrappedBasketLinks));
                    onSuccess.Invoke((TReturnType)emptyBasketLinks);
                    return;
                }
                
                var response = JsonConvert.DeserializeObject<TReturnType>(body);
                if (response == null)
                {
                    throw new JsonSerializationException("Response body was null, expected JSON");
                }
                onSuccess.Invoke(response);
            }
            catch (JsonSerializationException e)
            {
                onServerError.Invoke(new ServerError(statusCode, e.Message + " | " + body));
            }
        }
    }
}