using System.Text.Json.Serialization;

namespace PeopleOData
{
    public class HomeAddress
    {
        [JsonPropertyName("Address")]
        public object Address { get; set; }

        [JsonPropertyName("City")]
        public object City { get; set; }
    }
}
