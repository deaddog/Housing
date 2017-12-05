using Flurl;
using Flurl.Http;
using Housing.Models;
using Housing.Services.Dawa.InnerModels;
using System.Linq;
using System.Threading.Tasks;

namespace Housing.Services.Dawa
{
    public class DawaRepository : IDawaRepository
    {
        public async Task<DawaAddress[]> SearchForHouse(AddressLocation address)
        {
            var response = await $"https://dawa.aws.dk/adgangsadresser/autocomplete"
                .SetQueryParam("q", GetParameter(address))
                .GetJsonAsync<AddressElement[]>();

            return response.Select(a => a.Address).ToArray();
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
