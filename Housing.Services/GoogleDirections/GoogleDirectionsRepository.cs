using Flurl;
using Flurl.Http;
using Housing.Models;
using System;
using System.Threading.Tasks;

namespace Housing.Services.GoogleDirections
{
    public class GoogleDirectionsRepository : IGoogleDirectionsRepository
    {
        private readonly string _googleApiKey;

        public GoogleDirectionsRepository(string googleApiKey)
        {
            _googleApiKey = googleApiKey ?? throw new ArgumentNullException(nameof(googleApiKey));
        }

        public async Task<Distance> GetDistance(string origin, string destination)
        {
            var response = await "https://maps.googleapis.com/maps/api/directions/json"
                .SetQueryParam("origin", origin)
                .SetQueryParam("destination", destination)
                .SetQueryParam("key", _googleApiKey)
                .SetQueryParam("units", "metric")
                .SetQueryParam("language", "da")
                .GetJsonAsync<InnerModels.Response>()
                .ConfigureAwait(false);

            var leg = response.Routes[0].Legs[0];

            return new Distance(leg.Distance.Meters / 1000.0, TimeSpan.FromSeconds(leg.Duration.Seconds));
        }
    }
}
