using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PeopleOData
{

    public class People
    {
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }

        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("LastName")]
        public string LastName { get; set; }

        [JsonPropertyName("MiddleName")]
        public object MiddleName { get; set; }

        [JsonPropertyName("Gender")]
        public string Gender { get; set; }

        [JsonPropertyName("Age")]
        public object Age { get; set; }

        [JsonPropertyName("Emails")]
        public List<string> Emails { get; set; }

        [JsonPropertyName("FavoriteFeature")]
        public string FavoriteFeature { get; set; }

        [JsonPropertyName("Features")]
        public List<string> Features { get; set; }

        [JsonPropertyName("AddressInfo")]
        public List<AddressInfo> AddressInfo { get; set; }

        [JsonPropertyName("HomeAddress")]
        public HomeAddress HomeAddress { get; set; }

        [JsonPropertyName("@odata.type")]
        public string OdataType { get; set; }

        [JsonPropertyName("Budget")]
        public int? Budget { get; set; }

        [JsonPropertyName("BossOffice")]
        public object BossOffice { get; set; }

        [JsonPropertyName("Cost")]
        public int? Cost { get; set; }
    }

    public class PeopleData
    {
        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }
        [JsonPropertyName("value")]
        public List<People> Peoples { get; set; }
    }
}
