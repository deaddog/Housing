using System;

namespace Housing.Models
{
    public class Shop
    {
        public Shop(string name, AddressLocation address)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public string Name { get; }
        public AddressLocation Address { get; }
    }
}
