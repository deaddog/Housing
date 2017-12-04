using System;
using System.Text.RegularExpressions;

namespace Housing.Models
{
    public class AddressLocation
    {
        public static AddressLocation Parse(string address)
        {
            var addressMatch = Regex.Match(address, @"^(.*[^ ]) *, *(\d{4} .*)$");

            return Parse(addressMatch.Groups[1].Value, addressMatch.Groups[2].Value);
        }
        public static AddressLocation Parse(string streetAndTown, string postalCodeAndCity)
        {
            var streetMatch = Regex.Match(streetAndTown, "^(.*[^ ]) *, *([^ ].*)$");
            var cityMatch = Regex.Match(postalCodeAndCity, @"(\d+) *([^ ].*)");

            var postal = int.Parse(cityMatch.Groups[1].Value);
            var city = cityMatch.Groups[2].Value;

            if (streetMatch.Success)
                return new AddressLocation(streetMatch.Groups[1].Value, streetMatch.Groups[2].Value, postal, city);
            else
                return new AddressLocation(streetAndTown, postal, city);
        }

        public AddressLocation(string street, string town, int postalCode, string city)
        {
            Street = street ?? throw new ArgumentNullException(nameof(street));
            Town = town ?? throw new ArgumentNullException(nameof(town));
            PostalCode = postalCode;
            City = city ?? throw new ArgumentNullException(nameof(city));
        }
        public AddressLocation(string street, int postalCode, string city)
        {
            Street = street ?? throw new ArgumentNullException(nameof(street));
            Town = null;
            PostalCode = postalCode;
            City = city ?? throw new ArgumentNullException(nameof(city));
        }

        public string Street { get; }
        public string Town { get; }
        public int PostalCode { get; }
        public string City { get; }
    }
}
