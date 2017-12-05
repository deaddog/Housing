using Newtonsoft.Json;

namespace Housing.Services.Dawa.InnerModels
{
    internal class AddressElement
    {
        [JsonProperty(PropertyName = "tekst")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "adgangsadresse")]
        public DawaAddress Address { get; set; }
    }
}
