using System;

namespace Housing.Models
{
    public class AddressLocation
    {
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
