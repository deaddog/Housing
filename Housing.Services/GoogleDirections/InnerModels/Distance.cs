using Newtonsoft.Json;

namespace Housing.Services.GoogleDirections.InnerModels
{
    internal class Distance
    {
        [JsonProperty(PropertyName = "value")]
        public int Meters { get; set; }
    }
}
