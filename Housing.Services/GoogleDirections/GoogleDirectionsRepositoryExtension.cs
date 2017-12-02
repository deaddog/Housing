using Housing.Models;
using System.Globalization;
using System.Threading.Tasks;

namespace Housing.Services.GoogleDirections
{
    public static class GoogleDirectionsRepositoryExtension
    {
        public static Task<Distance> GetDistance(this IGoogleDirectionsRepository repository, AddressLocation origin, AddressLocation destination) => repository.GetDistance(GetParameter(origin), GetParameter(destination));
        public static Task<Distance> GetDistance(this IGoogleDirectionsRepository repository, GeoLocation origin, AddressLocation destination) => repository.GetDistance(GetParameter(origin), GetParameter(destination));
        public static Task<Distance> GetDistance(this IGoogleDirectionsRepository repository, GeoLocation origin, GeoLocation destination) => repository.GetDistance(GetParameter(origin), GetParameter(destination));
        public static Task<Distance> GetDistance(this IGoogleDirectionsRepository repository, AddressLocation origin, GeoLocation destination) => repository.GetDistance(GetParameter(origin), GetParameter(destination));

        private static string GetParameter(AddressLocation location)
        {
            if (location.Town != null)
                return $"{location.Street}, {location.Town}, {location.PostalCode} {location.City}";
            else
                return $"{location.Street}, {location.PostalCode} {location.City}";
        }
        private static string GetParameter(GeoLocation location)
        {
            return location.Latitude.ToString(CultureInfo.InvariantCulture) + "," + location.Longitude.ToString(CultureInfo.InvariantCulture);
        }
    }
}
