using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Housing.Models
{
    public class House
    {
        public House(
            Uri houseUrl,
            AddressLocation address,
            Area groundArea,
            Area houseArea,
            EnergyTypes energyType,
            DateTime forSaleDate,
            DateTime originalForSaleDate,
            IEnumerable<HouseImage> images,
            IEnumerable<InternetOption> internetOptions,
            IEnumerable<Shop> shops,
            IEnumerable<CategoryRating> ratings)
        {
            if (houseUrl.Scheme != Uri.UriSchemeHttp && houseUrl.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException($"House url should use {Uri.UriSchemeHttp} or {Uri.UriSchemeHttps}.", nameof(houseUrl));

            if (originalForSaleDate > forSaleDate)
                throw new ArgumentOutOfRangeException(nameof(forSaleDate), "Original for sale date cannot come before the current for sale date.");

            if (forSaleDate > DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(forSaleDate), "For sale date cannot be in the future.");
            if (originalForSaleDate > DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(originalForSaleDate), "Original for sale date cannot be in the future.");

            HouseUrl = houseUrl;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            GroundArea = groundArea ?? throw new ArgumentNullException(nameof(address));
            HouseArea = houseArea ?? throw new ArgumentNullException(nameof(address));
            EnergyType = energyType;
            ForSaleDate = forSaleDate;
            OriginalForSaleDate = originalForSaleDate;
            Images = new ReadOnlyCollection<HouseImage>(images?.ToList() ?? throw new ArgumentNullException(nameof(images)));
            InternetOptions = new ReadOnlyCollection<InternetOption>(internetOptions?.ToList() ?? throw new ArgumentNullException(nameof(internetOptions)));
            Shops = new ReadOnlyCollection<Shop>(shops?.ToList() ?? throw new ArgumentNullException(nameof(shops)));
            Ratings = new ReadOnlyCollection<CategoryRating>(ratings?.ToList() ?? throw new ArgumentNullException(nameof(ratings)));
        }

        public Uri HouseUrl { get; }
        public AddressLocation Address { get; }

        public Area GroundArea { get; }
        public Area HouseArea { get; }
        public EnergyTypes EnergyType { get; }

        public DateTime ForSaleDate { get; }
        public DateTime OriginalForSaleDate { get; }

        public ReadOnlyCollection<HouseImage> Images { get; }
        public ReadOnlyCollection<InternetOption> InternetOptions { get; }
        public ReadOnlyCollection<Shop> Shops { get; }

        public ReadOnlyCollection<CategoryRating> Ratings { get; }
    }
}
