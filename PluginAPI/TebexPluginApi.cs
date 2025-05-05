using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tebex.Common;
using Tebex.Plugin;

namespace Tebex.PluginAPI
{
    public class TebexPluginApi
    {
        public static readonly string PluginApiBase = "https://plugin.tebex.io/";
        private PluginAdapter Adapter { get; set; }
        private string SecretKey { get; set; }

#pragma warning disable CS8618, CS9264
        private TebexPluginApi() { } // properties assigned in Initialize()
#pragma warning restore CS8618, CS9264
        
        public static TebexPluginApi Initialize(PluginAdapter pluginAdapter, string secretKey)
        {
            var api = new TebexPluginApi
            {
                Adapter = pluginAdapter,
                SecretKey = secretKey
            };
            return api;
        }

        private Task Send<T>(string endpoint, string requestBody, HttpVerb method, Action<T> onSuccess, Action<PluginApiError>? onPluginApiError, Action<ServerError>? onServerError)
        {
            onServerError ??= Adapter.DefaultServerError;
            onPluginApiError ??= Adapter.DefaultPluginError;
            
            return Adapter.Send<T>(SecretKey,PluginApiBase + endpoint, requestBody, method, (code, responseBody) =>
            {
                try
                {
                    if (code == 204) // No content
                    {
                        onSuccess.Invoke(default!);
                    }
                    
                    var jsonObj = JsonConvert.DeserializeObject<T>(responseBody);
                    if (jsonObj == null)
                    {
                        throw new JsonException("Response body expected JSON, but was null");
                    }
                    
                    onSuccess.Invoke(jsonObj);
                }
                catch (JsonException e)
                {
                    onServerError.Invoke(new ServerError(code, e.Message + " | " + requestBody));
                }
            }, onPluginApiError, onServerError);
        }
        
        public Task SendJoinEvents(List<JoinEvent> events, Action<string> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("events", JsonConvert.SerializeObject(events), HttpVerb.POST, onSuccess, onPluginApiError, onServerError);
        }
        
        public Task GetInformation(Action<Store> storeInfo, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("information", "", HttpVerb.GET, storeInfo, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Retrieves a list of players with commands queued for execution the next time they log in to the game server.
        /// The response includes any offline commands to be processed and the time, in seconds, to wait before the next queue check.
        /// It is imperative to adhere to the `next_check` response to avoid potential penalties, such as revocation of the secret key or an IP ban from the API.
        /// </summary>
        /// <param name="onSuccess">The callback function invoked upon a successful response, providing the command queue details.</param>
        /// <param name="onPluginApiError">The callback function invoked when a plugin API error occurs.</param>
        /// <param name="onServerError">The callback function invoked when a server error occurs.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of retrieving the command queue data.</returns>
        public Task GetCommandQueue(Action<CommandQueueResponse> onSuccess, Action<PluginApiError> onPluginApiError,
            Action<ServerError> onServerError)
        {
            return Send("queue", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets commands that can be executed on the player even if they are offline.
        /// </summary>
        public Task GetOfflineCommands(Action<OfflineCommandsResponse> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("queue/offline-commands", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets commands that can be executed on the player if they are online.
        /// </summary>
        public Task GetOnlineCommands(int playerId, Action<OnlineCommandsResponse> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"queue/online-commands/{playerId}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Deletes one or more commands that have been executed on the game server.
        /// An empty response with the status code of 204 No Content will be returned on completion.
        /// </summary>
        public Task DeleteCommands(int[] ids, Action<EmptyResponse> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            var payload = new DeleteCommandsPayload
            {
                Ids = ids
            };
            return Send("queue", JsonConvert.SerializeObject(payload), HttpVerb.DELETE, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Get the categories and packages which should be displayed to players in game. The returned order of this endpoint
        /// does not reflect the desired order of the category/packages - please order based on the order object.
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onPluginApiError"></param>
        /// <param name="onServerError"></param>
        /// <returns></returns>
        public Task GetListings(Action<ListingsResponse> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("listing", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Get a list of all packages on the webstore. Pass verbose=true to include descriptions of the packages.
        /// API returns a list of JSON encoded Packages.
        /// </summary>
        /// <param name="verbose"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onPluginApiError"></param>
        /// <param name="onServerError"></param>
        /// <returns></returns>
        public Task GetAllPackages(bool verbose, Action<List<Package>> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send(verbose ? "packages?verbose=true" : "packages", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }
        
        /// <summary>
        /// Gets a specific package from the webstore by its ID. Returns JSON-encoded Package object.
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onPluginApiError"></param>
        /// <param name="onServerError"></param>
        /// <returns></returns>
        public Task GetPackage(int packageId, Action<Package> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"package/{packageId}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Retrieves all community goals from the account.
        /// </summary>
        public Task GetAllCommunityGoals(Action<List<CommunityGoal>> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("community_goals", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Retrieves a specific community goal.
        /// </summary>
        public Task GetCommunityGoal(int goalId, Action<CommunityGoal> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"community_goals/{goalId}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }
        
        /// <summary>
        /// Retrieve the latest payments (up to a maximum of 100) made on the webstore.
        /// </summary>
        /// <returns></returns>
        public Task GetAllPayments(int limit, Action<List<PaymentDetails>> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            if (limit <= 0)
            {
                limit = 1;
            }
            
            if (limit > 100)
            {
                limit = 100;
            }
            return Send($"payments?limit={limit}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Return all payments as a page, in the given order.
        /// </summary>
        public Task GetAllPaymentsPaginated(int pageNumber, Action<PaginatedPaymentInfo> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"payments?paged={pageNumber}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Retrieve a payment by its transaction ID
        /// </summary>
        public Task GetPayment(string transactionId, Action<PaymentDetails> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"payments/{transactionId}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }
        
        /// <summary>
        /// Creates a URL which will take the player to a checkout area in order to purchase the given item.
        /// </summary>
        public Task CreateCheckoutUrl(int packageId, string username, Action<CheckoutResponse> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            var payload = new CreateCheckoutPayload
            {
                PackageId = packageId,
                Username = username
            };
            return Send("checkout", JsonConvert.SerializeObject(payload), HttpVerb.POST, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets all gift cards on the account.
        /// </summary>
        public Task GetAllGiftCards(Action<WrappedGiftCards> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("gift-cards", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets a gift card by its id.
        /// </summary>
        public Task GetGiftCard(int giftCardId, Action<WrappedGiftCard> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"gift-cards/{giftCardId}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Creates a gift card
        /// </summary>
        public Task CreateGiftCard(DateTime expiresAt, string note, int amount, Action<GiftCard> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            var payload = new CreateGiftCardPayload
            {
                ExpiresAt = expiresAt,
                Note = note,
                Amount = amount
            };
            
            return Send("gift-cards", JsonConvert.SerializeObject(payload), HttpVerb.POST, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Voids a gift card on the account by its id
        /// </summary>
        public Task VoidGiftCard(int giftCardId, Action<WrappedGiftCard> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"gift-cards/{giftCardId}", "", HttpVerb.DELETE, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Adds an amount to a gift card by its id
        /// </summary>
        public Task TopUpGiftCard(int giftCardId, double amount, Action<WrappedGiftCard> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            var payload = new TopUpGiftCardPayload
            {
                Amount = $"{amount}"
            };
            return Send($"gift-cards/{giftCardId}", JsonConvert.SerializeObject(payload), HttpVerb.PUT, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets all coupons on the account.
        /// </summary>
        public Task GetAllCoupons(Action<CouponPage> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("coupons", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets a coupon by its id from the store.
        /// </summary>
        public Task GetCouponById(string couponId, Action<WrappedCoupon> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"coupons/{couponId}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets all users banned from the webstore.
        /// </summary>
        public Task GetAllBans(Action<WrappedBans> onSuccess, Action<PluginApiError> onPluginApiError,
            Action<ServerError> onServerError)
        {
            return Send("bans", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Creates a new ban for a user.
        /// </summary>
        public Task CreateBan(string reason, string ip, string userId, Action<WrappedBan> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            var payload = new CreateBanPayload
            {
                Reason = reason,
                IP = ip,
                User = userId
            };
            return Send("bans", JsonConvert.SerializeObject(payload), HttpVerb.POST, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Gets all sales on the account.
        /// </summary>
        public Task GetAllSales(Action<WrappedSales> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send("sales", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }
        
        /// <summary>
        /// Gets a user by their uuid or username using the lookup function
        /// </summary>
        public Task Lookup(string uuidOrUsername, Action<UserLookupResponse> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"user/{uuidOrUsername}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }
        
        /// <summary>
        /// Return a list of all active (non-expired) packages that a customer has purchased.
        /// If packageId is provided, filter down to a single package ID, if you want to check if a specific package has been purchased.
        /// </summary>
        public Task GetActivePackagesForCustomer(string playerId, Action<List<ActivePackage>> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"player/{playerId}/packages", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }

        /// <summary>
        /// Checks if a specific package is active for a specific user.
        /// </summary>
        public Task GetActivePackageById(string userId, int packageId, Action<List<ActivePackage>> onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError)
        {
            return Send($"player/{userId}/packages?package={packageId}", "", HttpVerb.GET, onSuccess, onPluginApiError, onServerError);
        }
    }
}