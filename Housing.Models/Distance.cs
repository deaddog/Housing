using System;

namespace Housing.Models
{
    public class Distance
    {
        public Distance(double distanceKm, TimeSpan duration)
        {
            if (distanceKm < 0)
                throw new ArgumentOutOfRangeException(nameof(distanceKm), "Distance cannot be negative.");

            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration), "Distance duration cannot be negative.");

            DistanceKm = distanceKm;
            Duration = duration;
        }

        public double DistanceKm { get; }
        public TimeSpan Duration { get; }
    }
}
