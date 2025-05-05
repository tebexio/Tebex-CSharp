using Newtonsoft.Json;

namespace Tebex.HeadlessAPI
{
    /// <summary>
    /// A code that triggers affiliation of the basket with a platform creator.
    /// </summary>
    public class CreatorCode
    {
        [JsonProperty("creator_code")]
        public string Code { get; private set; }

        public CreatorCode(string code)
        {
            Code = code;
        }
    }

    /// <summary>
    /// A gift card that can be applied to a basket to discount it.
    /// </summary>
    public class GiftCard
    {
        [JsonProperty("card_number")]
        public string CardNumber { get; private set; }

        public GiftCard(string cardNumber)
        {
            CardNumber = cardNumber;
        }
    }
}