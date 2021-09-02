using System.Text.Json.Serialization;
namespace PeopleOData
{
    public class AddressInfo
    {
        [JsonPropertyName("Address")]
        public string Address { get; set; }

        [JsonPropertyName("City")]
        public City City { get; set; }
    }
}
