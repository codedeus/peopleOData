using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PeopleOData
{

    public class BaseData
    {
        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }
        public List<BaseDataValue> value { get; set; }
    }

}
