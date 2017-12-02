using Newtonsoft.Json;
using System;

namespace Housing.Services.Dawa
{
    public class DawaAddress
    {
        [JsonProperty(PropertyName = "id")]
        public Guid AddressId { get; set; }
        [JsonProperty(PropertyName = "href")]
        public Uri HRef { get; set; }
        [JsonProperty(PropertyName = "vejnavn")]
        public string StreetName { get; set; }
        [JsonProperty(PropertyName = "husnr")]
        public string HouseNumber { get; set; }
        [JsonProperty(PropertyName = "supplerendebynavn")]
        public object Town { get; set; }
        [JsonProperty(PropertyName = "postnr")]
        public int PostalCode { get; set; }
        [JsonProperty(PropertyName = "postnrnavn")]
        public string City { get; set; }
    }
}
