using Newtonsoft.Json;

namespace Lab3.Models
{
    public class User
    {
        [JsonProperty("userId")]
        public string Id { get; set; }

        [JsonProperty("userFirstName")]
        public string FirstName { get; set; }

        [JsonProperty("userSecondName")]
        public string SecondName { get; set; }
    }
}