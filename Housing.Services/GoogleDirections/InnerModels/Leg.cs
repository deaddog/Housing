using Newtonsoft.Json;

namespace Housing.Services.GoogleDirections.InnerModels
{
    internal class Leg
    {
        [JsonProperty(PropertyName = "duration")]
        public Duration Duration { get; set; }
        [JsonProperty(PropertyName = "distance")]
        public Distance Distance { get; set; }
    }
}
