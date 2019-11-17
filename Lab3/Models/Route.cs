using Newtonsoft.Json;

namespace Lab3.Models
{
    public class Route
    {
        [JsonProperty("routeId")]
        public string Id { get; set; }

        [JsonProperty("routeFrom")]
        public string From { get; set; }

        [JsonProperty("routeWhere")]
        public string To { get; set; }

        [JsonProperty("routeDate")]
        public string Date { get; set; }

        [JsonProperty("routeTime")]
        public string Time { get; set; }

        [JsonProperty("routePrice")]
        public string Price { get; set; }
    }
}
