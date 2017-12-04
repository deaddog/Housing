using Flurl;
using Flurl.Http;
using Housing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Housing.Services.eTilbudsavis
{
    public class eTilbudsavisRepository : IeTilbudsavisRepository
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _root = "https://api.etilbudsavis.dk/v2";

        private readonly SHA256Managed _sha = new SHA256Managed();

        private readonly string _sessionToken;

        public eTilbudsavisRepository(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _sessionToken = GetSessionToken().Result;
        }

        private async Task<string> GetSessionToken()
        {
            var response = await $"{_root}/sessions"
                .PostJsonAsync(new { api_key = _apiKey })
                .ReceiveJson()
                .ConfigureAwait(false);

            return response.token;
        }
        private string GetSignature()
        {
            var hash = _sha.ComputeHash(Encoding.UTF8.GetBytes(_apiSecret + _sessionToken));
            var hex = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return hex;
        }

        public async Task<List<Dealer>> GetDealers()
        {
            var result = new List<Dealer>();

            int offset = 0;

            do
            {
                var response = await $"{_root}/dealers"
                    .SetQueryParam("offset", offset)
                    .WithHeader("X-Token", _sessionToken)
                    .WithHeader("X-Signature", GetSignature())
                    .GetJsonListAsync()
                    .ConfigureAwait(false);

                if (response.Count == 0)
                    break;

                offset += 24;

                result.AddRange(response.Where(x => x.country.id == "DK").Select(x => new Dealer(x.id as string, x.name as string)));
            } while (true);

            return result;
        }
        public async Task<List<Shop>> GetShops(GeoLocation location, IEnumerable<string> dealerIds)
        {
            var shopQuery = $"{_root}/stores"
                .SetQueryParam("r_lat", location.Latitude)
                .SetQueryParam("r_lng", location.Longitude);

            if (dealerIds?.Any() ?? false)
                shopQuery = shopQuery.SetQueryParam("dealer_ids", string.Join(",", dealerIds));

            var shops = await shopQuery
                .SetQueryParam("order_by", "distance")
                .WithHeader("X-Token", _sessionToken)
                .WithHeader("X-Signature", GetSignature())
                .GetJsonListAsync()
                .ConfigureAwait(false);

            return shops.Select(x => new Shop(
                x.Branding.Name,
                new AddressLocation(
                    x.Street,
                    x.ZipCode,
                    x.City
                )
            )).ToList();
        }
    }
}
