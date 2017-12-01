using Newtonsoft.Json;

namespace Housing.Services.GoogleDirections.InnerModels
{
    internal class Duration
    {
        [JsonProperty(PropertyName = "value")]
        public int Seconds { get; set; }
    }
}
