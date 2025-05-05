using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tebex.PluginAPI
{
    
    public class PlayerPaymentInfo
    {
        [JsonProperty("txn_id")] public string TransactionId { get; private set; } = string.Empty;

        [JsonProperty("time")] public long Time { get; private set; }

        [JsonProperty("price")] public double Price { get; private set; }

        [JsonProperty("currency")] public string Currency { get; private set; } = string.Empty;

        [JsonProperty("status")] public string Status { get; private set; } = string.Empty;
    }
    
    public class PurchaseRecordPackageInfo
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")] public string Name { get; private set; } = string.Empty;
    }

    public class CustomerPackagePurchaseRecord
    {
        [JsonProperty("txn_id")]
        public string TransactionId { get; private set; } = string.Empty;

        [JsonProperty("date")]
        public DateTime Date { get; private set; }

        [JsonProperty("quantity")]
        public int Quantity { get; private set; }

        [JsonProperty("package")]
        public PurchaseRecordPackageInfo PurchaseRecordPackage { get; private set; } = new PurchaseRecordPackageInfo();
    }
    
    public class GatewayInfo
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;
    }

    public class PaidPlayer
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty("uuid")]
        public string Uuid { get; private set; } = string.Empty;
    }

    public class PackageInfo
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")] public string Name { get; private set; } = string.Empty;
    }

    public class NoteInfo
    {
        [JsonProperty("created_at")]
        public string CreatedAt { get; private set; } = string.Empty;

        [JsonProperty("note")] public string Note { get; private set; } = string.Empty;
    }
    
    public class PaymentDetails
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("amount")]
        public double Amount { get; private set; }

        [JsonProperty("date")]
        public DateTime Date { get; private set; }

        [JsonProperty("currency")]
        public Currency Currency { get; private set; } = new Currency();

        [JsonProperty("gateway")]
        public GatewayInfo Gateway { get; private set; } = new GatewayInfo();

        [JsonProperty("status")]
        public string Status { get; private set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; private set; } = string.Empty;

        [JsonProperty("player")]
        public PaidPlayer Player { get; private set; } = new PaidPlayer();

        [JsonProperty("packages")]
        public List<PackageInfo> Packages { get; private set; } = new List<PackageInfo>();

        [JsonProperty("notes")]
        public List<NoteInfo> Notes { get; private set; } = new List<NoteInfo>();

        [JsonProperty("creator_code")]
        public string? CreatorCode { get; private set; }
    }

    public class PaginatedPaymentInfo
    {
        [JsonProperty("total")]
        public int Total = -1;
        [JsonProperty("per_page")]
        public int PerPage = -1;
        [JsonProperty("current_page")]
        public int CurrentPage = -1;
        [JsonProperty("last_page")]
        public int LastPage = -1;
        [JsonProperty("next_page_url")]
        public string? NextPageUrl = string.Empty;
        [JsonProperty("prev_page_url")]
        public string? PreviousPageUrl = string.Empty;
        [JsonProperty("from")]
        public int From = -1;
        [JsonProperty("to")]
        public int To = -1;
        
        [JsonProperty("data")]
        public List<PaymentDetails> Data { get; private set; } = new List<PaymentDetails>();
    }
}