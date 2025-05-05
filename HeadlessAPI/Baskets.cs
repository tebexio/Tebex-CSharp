using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// The primary Basket used in the Headless API.
    /// </summary>
    public class Basket
    {
        [JsonProperty("ident")]
        public string Ident { get; private set; } = string.Empty;

        [JsonProperty("complete")]
        public bool Complete { get; private set; }

        [JsonProperty("email")]
        public string Email { get; private set; } = string.Empty;

        [JsonProperty("username")]
        public string Username { get; private set; } = string.Empty;

        [JsonProperty("coupons")]
        public List<BasketCoupon> Coupons { get; private set; } = new List<BasketCoupon>();

        [JsonProperty("gift_cards")]
        public List<BasketGiftCard> GiftCards { get; private set; } = new List<BasketGiftCard>();

        [JsonProperty("creator_code")]
        public string CreatorCode { get; private set; } = string.Empty;

        [JsonProperty("cancel_url")]
        public string CancelUrl { get; private set; } = string.Empty;

        [JsonProperty("complete_url")]
        public string CompleteUrl { get; private set; } = string.Empty;

        [JsonProperty("complete_auto_redirect")]
        public bool CompleteAutoRedirect { get; private set; }

        [JsonProperty("country")]
        public string Country { get; private set; } = string.Empty;

        [JsonProperty("ip")]
        public string Ip { get; private set; } = string.Empty;

        [JsonProperty("username_id")]
        public int? UsernameId { get; private set; }

        [JsonProperty("base_price")]
        public float BasePrice { get; private set; }

        [JsonProperty("sales_tax")]
        public float SalesTax { get; private set; }

        [JsonProperty("total_price")]
        public float TotalPrice { get; private set; }

        [JsonProperty("currency")]
        public string Currency { get; private set; } = string.Empty;

        [JsonProperty("packages")]
        public List<BasketPackage> Packages { get; private set; } = new List<BasketPackage>();

        [JsonProperty("custom")]
        public Dictionary<string, string> Custom { get; private set; } = new Dictionary<string, string>();
        
        [JsonProperty("links")]
        public BasketLinks Links { get; private set; } = new BasketLinks();
    }
    
    /// <summary>
    /// Information about the basket's revenue share.
    /// </summary>
    public class BasketRevenueShare
    {
        [JsonProperty("wallet_ref")]
        public string WalletRef { get; private set; } = string.Empty;

        [JsonProperty("amount")]
        public float Amount { get; private set; } = -1.0f;

        [JsonProperty("gateway_fee_percent")]
        public int GatewayFeePercent { get; private set; } = -1;
    }

    /// <summary>
    /// Qualities about a package within a basket.
    /// </summary>
    public class InnerPackageMeta
    {
        [JsonProperty("quantity")]
        public int Quantity { get; private set; } = -1;

        [JsonProperty("price")]
        public float Price { get; private set; } = -1.0f;

        [JsonProperty("gift_username_id")]
        public string GiftUsernameId { get; private set; } = string.Empty;

        [JsonProperty("gift_username")]
        public string GiftUsername { get; private set; } = string.Empty;

        [JsonProperty("gift_image")]
        public string GiftImage { get; private set; } = string.Empty;
    }

    /// <summary>
    /// A package currently in a Basket.
    /// </summary>
    public class BasketPackage
    {
        [JsonProperty("id")]
        public int Id { get; private set; } = -1;

        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; private set; } = string.Empty;

        [JsonProperty("in_basket")]
        public InnerPackageMeta In { get; private set; } = new InnerPackageMeta();
    }

    /// <summary>
    /// A coupon placed on a Basket.
    /// </summary>
    public class BasketCoupon
    {
        [JsonProperty("coupon_code")]
        public string CouponCode { get; private set; }
        
        public BasketCoupon(string couponCode)
        {
            CouponCode = couponCode;
        }
    }

    /// <summary>
    /// A gift card placed on a Basket.
    /// </summary>
    public class BasketGiftCard
    {
        [JsonProperty("card_number")]
        public string CardNumber { get; private set; } = string.Empty;
    }

    /// <summary>
    /// A Basket's external links for payment and checkout.
    /// </summary>
    public class BasketLinks
    {
        [JsonProperty("payment")]
        public string Payment { get; private set; } = string.Empty;

        [JsonProperty("checkout")]
        public string Checkout { get; private set; } = string.Empty;
    }

    /// <summary>
    /// A link a user may use to login and authorize their Basket.
    /// </summary>
    public class BasketAuthLink
    {
        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty("url")]
        public string Url { get; private set; } = string.Empty;
    }

    /// <summary>
    /// A data container class that wraps a single instance of a Basket.
    /// </summary>
    public class WrappedBasket
    {
        [JsonProperty("data")]
        public Basket Data { get; private set; } = new Basket();
    }

    /// <summary>
    /// An array of authorization links for a Basket.
    /// </summary>
    public class WrappedBasketLinks
    {
        public List<BasketAuthLink> LinksArray { get; private set; } = new List<BasketAuthLink>();
    }
    
}