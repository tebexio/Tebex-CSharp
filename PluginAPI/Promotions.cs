using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    public class WrappedGiftCards
    {
        [JsonProperty("data")]
        public List<GiftCard> Data = new List<GiftCard>();
    }

    public class WrappedGiftCard
    {
        [JsonProperty("data")]
        public GiftCard Data = new GiftCard();
    }
    
    public class GiftCard
    {
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("code")]
        public string Code { get; private set; } = string.Empty;
        [JsonProperty("balance")] 
        public GiftCardBalance Balance { get; private set; } = new GiftCardBalance();
        
        [JsonProperty("note")]
        public string Note { get; private set; } = string.Empty;
        
        [JsonProperty("void")]
        public bool Void { get; private set; }
        
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; private set; }
        
        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; private set; }
    }

    public class GiftCardBalance
    {
        [JsonProperty("starting")] public double Starting { get; private set; } = -1.0d;
        [JsonProperty("remaining")] public double Remaining { get; private set; } = -1.0d;
        [JsonProperty("currency")] public string Currency { get; private set; } = string.Empty;
    }
    
    public class WrappedCoupon
    {
        [JsonProperty("data")]
        public Coupon Data = new Coupon();
    }
    
    public class Coupon
    {
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("code")]
        public string Code { get; private set; } = string.Empty;
        [JsonProperty("effective")]
        public PromotionEffectiveListings Effective { get; private set; } = new PromotionEffectiveListings();
        [JsonProperty("discount")]
        public PromotionDiscount Discount { get; private set; } = new PromotionDiscount();
        [JsonProperty("expire")]
        public CouponExpiry Expire { get; private set; } = new CouponExpiry();
        [JsonProperty("basket_type")]
        public string BasketType { get; private set; } = string.Empty;
        [JsonProperty("start_date")]
        public DateTime StartDate { get; private set; }
        [JsonProperty("user_limit")]
        public int UserLimit { get; private set; }
        [JsonProperty("minimum")]
        public int Minimum { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; } = string.Empty;
        [JsonProperty("note")]
        public string Note { get; private set; } = string.Empty;
    }
    
    public class PromotionDiscount
    {
        [JsonProperty("type")]
        public string Type { get; private set; } = string.Empty;
        [JsonProperty("percentage")]
        public double Percentage { get; private set; }
        [JsonProperty("value")]
        public double Value { get; private set; }
    }

    public class CouponExpiry
    {
        [JsonProperty("redeem_unlimited")]
        public bool RedeemUnlimited { get; private set; }
        [JsonProperty("expire_never")]
        public bool ExpireNever { get; private set; }
        [JsonProperty("limit")]
        public int Limit { get; private set; }
        [JsonProperty("date")]
        public DateTime? Date { get; private set; }
    }

    public class PromotionEffectiveListings
    {
        [JsonProperty("type")]
        public string Type { get; private set; } = string.Empty;
        [JsonProperty("packages")]
        public List<int> Packages { get; private set; } = new List<int>();
        [JsonProperty("categories")]
        public List<int> Categories { get; private set; } = new List<int>();
    }

    public class CouponPage
    {
        [JsonProperty("pagination")]
        public CouponPagination Pagination { get; private set; } = new CouponPagination();
        
        [JsonProperty("data")]
        public List<Coupon> Data { get; private set; } = new List<Coupon>();
    }

    public class CouponPagination
    {
        [JsonProperty("totalResults")]
        public int TotalResults { get; private set; }
        [JsonProperty("currentPage")]
        public int CurrentPage { get; private set; }
        [JsonProperty("lastPage")]
        public int LastPage { get; private set; }
        [JsonProperty("previous")]
        public string? Previous { get; private set; }
        [JsonProperty("next")]
        public string? Next { get; private set; }
    }

    public class WrappedSales
    {
        [JsonProperty("data")]
        public List<Sale> Data = new List<Sale>();
    }
    
    public class Sale
    {
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;
        [JsonProperty("effective")]
        public PromotionEffectiveListings Effective { get; private set; } = new PromotionEffectiveListings();
        [JsonProperty("discount")]
        public PromotionDiscount Discount { get; private set; } = new PromotionDiscount();
        [JsonProperty("start")]
        public int Start { get; private set; }
        [JsonProperty("expire")]
        public int Expire { get; private set; }
        [JsonProperty("order")]
        public int Order { get; private set; }
    }

    public class CommunityGoal
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; private set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [JsonProperty("account")]
        public int Account { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; private set; } = string.Empty;

        [JsonProperty("image")]
        public string Image { get; private set; } = string.Empty;

        [JsonProperty("target")]
        public double Target { get; private set; }

        [JsonProperty("current")]
        public double Current { get; private set; }

        [JsonProperty("repeatable")]
        public int Repeatable { get; private set; }

        [JsonProperty("last_achieved")]
        public DateTime? LastAchieved { get; private set; }

        [JsonProperty("times_achieved")]
        public int TimesAchieved { get; private set; }

        [JsonProperty("status")]
        public string Status { get; private set; } = string.Empty;

        [JsonProperty("sale")]
        public int Sale { get; private set; }
    }
    
    public class Ban
    {
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("time")]
        public DateTime Time { get; private set; }
        [JsonProperty("ip")]
        public string Ip { get; private set; } = string.Empty;
        [JsonProperty("payment_email")]
        public string? PaymentEmail { get; private set; }
        [JsonProperty("reason")]
        public string Reason { get; private set; } = string.Empty;
        [JsonProperty("user")]
        public BanUserInfo User { get; private set; } = new BanUserInfo();
    }

    public class BanUserInfo
    {
        [JsonProperty("ign")]
        public string Ign = string.Empty;
        [JsonProperty("uuid")]
        public string Uuid = string.Empty;
    }
    public class WrappedBans
    {
        [JsonProperty("data")]
        public List<Ban> Data = new List<Ban>();
    }

    public class WrappedBan
    {
        [JsonProperty("data")]
        public Ban Data = new Ban();
    }
}