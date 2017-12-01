using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Housing.Services.GoogleDirections.InnerModels
{
    internal class Route
    {
        [JsonProperty(PropertyName = "legs")]
        public ReadOnlyCollection<Leg> Legs { get; set; }
    }
}
