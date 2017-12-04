using Flurl.Http;
using Housing.Models;
using Housing.Scraping;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Housing.Services.Boligsiden
{
    public class BoligsidenRepository : IBoligsidenRepository
    {
        public async Task<BoligsidenHouseDetails> GetHouseDetailsAsync(AddressLocation address)
        {
            var houseId = await GetHouseId(address);

            var doc = await $"https://www.boligsiden.dk/salg/{houseId}"
                .GetHtmlAsync().ConfigureAwait(false);

            var detailsJson = doc.DocumentNode
                .SelectNodes("//script")
                .FirstOrDefault(x => x.ChildNodes.Count == 1 && x.ChildNodes[0].InnerHtml.TrimStart().StartsWith("bs.page.initPropertyPresentation"))
                .InnerHtml
                .Trim()
                .MatchRegex(@"^bs\.page\.initPropertyPresentation\((?<obj>.*)\);$")
                .Groups["obj"]
                .Value;

            var detailsObj = JToken.Parse(detailsJson);
            var prop = detailsObj["property"];

            var periodDays = prop.GetInt32("salesPeriod");
            var periodTotalDays = prop.GetNullableInt32("salesPeriodTotal");

            var location = prop["mapPosition"]["latLng"];
            var coordinates = new GeoLocation(location.Value<double>("lat"), location.Value<double>("lng"));

            return new BoligsidenHouseDetails(
                houseId,
                TimeSpan.FromDays(periodDays),
                periodTotalDays.HasValue ? TimeSpan.FromDays(periodTotalDays.Value) : (TimeSpan?)null,
                coordinates
                );
        }

        private async Task<int> GetHouseId(AddressLocation location)
        {
            string addressQuery;
            if (location.Town != null)
                addressQuery = $"{location.Street}, {location.Town}, {location.PostalCode} {location.City}";
            else
                addressQuery = $"{location.Street}, {location.PostalCode} {location.City}";

            var response = await $"https://www.boligsiden.dk/propertysearch/savequicksearch?query={addressQuery}&itemTypes=100|200|300|400|500|600|700|800|900&completionType=8&itemType=Property"
                .GetJsonAsync<JToken>();

            if (response["type"].Value<string>() == "SUGGESTIONLIST" && location.Town != null)
                return await GetHouseId(new AddressLocation(location.Street, location.PostalCode, location.City));

            if (response["url"] == null)
                throw new Exception("No url property in Boligsiden response");

            var url = response.Value<string>("url");
            var match = Regex.Match(url, @"/salg/(?<id>\d+)");

            if (!match.Success)
                throw new Exception("No house id could be extracted from Boligsiden url: " + url);

            return int.Parse(match.Groups["id"].Value);
        }
    }
}
