using Newtonsoft.Json;

namespace Lab3.Models
{
    public class User
    {
        [JsonProperty("userId")]
        public string Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("secondName")]
        public string SecondName { get; set; }
    }
}