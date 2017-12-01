using Housing.Models;
using System;

namespace Housing.Services.Boligsiden
{
    public class BoligsidenHouseDetails
    {
        public BoligsidenHouseDetails(int boligsidenId, TimeSpan salesPeriod, TimeSpan? salesPeriodTotal, GeoLocation location)
        {
            BoligsidenId = boligsidenId;
            SalesPeriod = salesPeriod;
            SalesPeriodTotal = salesPeriodTotal;
            Location = location;
        }

        public int BoligsidenId { get; }

        public TimeSpan SalesPeriod { get; }
        public TimeSpan? SalesPeriodTotal { get; }

        public GeoLocation Location { get; }
    }
}
