using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Housing.Services.GoogleDirections.InnerModels
{
    internal class Response
    {
        [JsonProperty(PropertyName = "routes")]
        public ReadOnlyCollection<Route> Routes { get; set; }
    }
}
