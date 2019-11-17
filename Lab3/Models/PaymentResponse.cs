using Newtonsoft.Json;

namespace Lab3.Models
{
    public class PaymentResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }
    }
}