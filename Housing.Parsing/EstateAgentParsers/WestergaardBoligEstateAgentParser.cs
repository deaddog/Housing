using Flurl.Http;
using Housing.Models;
using Housing.Scraping;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Housing.Parsing.EstateAgentParsers
{
    class WestergaardBoligEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "westergaardbolig.dk";

        private Dictionary<string, HtmlNode> GetDetailNodes(HtmlDocument document)
        {
            var dictionary = new Dictionary<string, HtmlNode>(StringComparer.OrdinalIgnoreCase);

            var rows = document.DocumentNode
                  .SelectNodes("//div[@class='mpdiv1']/table/tr")
                  .ToList();

            for (int i = 0; i < rows.Count; i += 2)
            {
                var values = rows[i + 1].SelectNodes("./td");
                var headers = rows[i]
                    .SelectNodes("./td")
                    .Select(x => x.DecodedInnerHtml().Trim().Trim('.', ':'));

                foreach (var item in values.Zip(headers, (v, h) => (Node: v, Key: h)))
                    dictionary.Add(item.Key, item.Node);
            }

            return dictionary;
        }

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var adr = document.DocumentNode.SelectSingleNode("//div[@class='mpdiv1']/h2[1]").DecodedInnerHtml();
            var cityMatch = document.DocumentNode
                .SelectSingleNode("//div[@class='mpdiv1']/p[2]")
                .DecodedInnerHtml()
                .MatchRegex(@"^(?<zip>\d+), *(?<name>[^ ].*[^ ]) *•");

            var postalCode = int.Parse(cityMatch.Groups["zip"].Value);
            var city = cityMatch.Groups["name"].Value;

            return AddressLocation.Parse(adr, $"{postalCode} {city}");
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var detail = GetDetailNodes(document);

            return double.Parse(detail["kontant"].DecodedInnerHtml().Replace(".", ""));
        }
        public async Task<EnergyTypes> GetEnergyType(HtmlDocument document)
        {
            var detail = GetDetailNodes(document);
            var energyNode = detail["energimærke"].SelectSingleNode("./span");

            var energyType = energyNode
                .GetAttributeValue("class", null)
                .MatchRegex("^energysprite energy-(.*)$")
                .Groups[1].Value;

            if (!Enum.TryParse<EnergyTypes>(energyType, out var result))
                result = EnergyTypes.Unknown;

            return result;
        }

        public async Task<Area> GetGroundArea(HtmlDocument document)
        {
            var detail = GetDetailNodes(document);

            return new Area(double.Parse(detail["grund"].DecodedInnerHtml().Replace(".", "")), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var detail = GetDetailNodes(document);

            return new Area(double.Parse(detail["kvm"].DecodedInnerHtml().Replace(".", "")), AreaUnits.SquareMeter);
        }

        #region Classes for mapping images json response

        private class Image
        {
            public string image_id { get; set; }
            public string image_name { get; set; }
            public string jobtype { get; set; }
            public string masked_folder { get; set; }
            public string label { get; set; }
            public string check { get; set; }
        }

        private class Result
        {
            public List<Image> images { get; set; }
            public string uprojectid { get; set; }
            public string case_name { get; set; }
            public string clientid { get; set; }
            public string companyid { get; set; }
            public string customerid { get; set; }
        }

        private class ImagesResponse
        {
            public Result result { get; set; }
            public bool ready { get; set; }
        }

        #endregion

        public async Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document)
        {
            var houseId = document.DocumentNode.SelectSingleNode("//meta[@property='og:url']").GetAttributeValue("content", null);
            houseId = houseId.Substring(houseId.LastIndexOf('/') + 1);

            var response = await $"http://westergaardbolig.dk/sags/images"
                .PostUrlEncodedAsync(new { caseid = int.Parse(houseId) })
                .ReceiveJson<ImagesResponse>().ConfigureAwait(false);

            var result = response.result;
            var url = $"http://remote.zacmatrix.dk/wemask/uploads/clientid_{result.clientid}/companyid_{result.companyid}/customerid_{result.customerid}/case_{result.case_name}/projectid_{result.uprojectid}";

            return result.images.Select(x =>
            {
                var uri = new Uri($"{url}/jobtype_{x.jobtype}/{x.masked_folder}/hd/{x.image_name}");
                if (x.jobtype == "1") // 1 seems to refer to the floorplan. The rest is unknown.
                    return new HouseImage(uri, ImageTypes.Floorplan);
                else
                    return new HouseImage(uri);
            });
        }
    }
}
