using Flurl;
using Flurl.Http;
using Housing.Models;
using System.Threading.Tasks;

namespace Housing.Services.Dawa
{
    public class DawaRepository : IDawaRepository
    {
        public async Task<DawaAddress[]> SearchForHouse(AddressLocation address)
        {
            return await $"https://dawa.aws.dk/adgangsadresser/autocomplete"
                .SetQueryParam("q", GetParameter(address))
                .GetJsonAsync<DawaAddress[]>();
        }

        private static string GetParameter(AddressLocation address)
        {
            if (address.Town != null)
                return $"{address.Street}, {address.Town}, {address.PostalCode} {address.City}";
            else
                return $"{address.Street}, {address.PostalCode} {address.City}";
        }
    }
}
