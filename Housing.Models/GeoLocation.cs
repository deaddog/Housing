using System;

namespace Housing.Models
{
    public class GeoLocation
    {
        public GeoLocation(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 (90° S) and 90 (90° N).");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Longitude must be between -180 (180° W) and 180 (180° E).");

            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }
        public double Longitude { get; }
    }
}
