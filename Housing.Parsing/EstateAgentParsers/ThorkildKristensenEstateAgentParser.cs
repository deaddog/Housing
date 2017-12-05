using Housing.Models;
using Housing.Scraping;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Housing.Parsing.EstateAgentParsers
{
    class ThorkildKristensenEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "thorkildkristensen.dk";

        private Dictionary<string, HtmlNode> GetDetailNodes(HtmlDocument document)
        {
            return document.DocumentNode
                  .SelectNodes("//ul[@class='case-data']/li")
                  .Select(x =>
                  {
                      var spans = x.SelectNodes("./span");
                      if (spans.Count != 2)
                          return (Key: null, Node: null);

                      var key = spans[0].DecodedInnerHtml().ToLower();
                      var value = spans[1];

                      return (Key: key, Node: value);
                  })
                  .Where(x => x.Key != null && x.Node != null)
                  .ToDictionary(x => x.Key, x => x.Node);
        }

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var titleText = document.DocumentNode
                .SelectSingleNode("//title")
                .DecodedInnerHtml()
                .Trim()
                .MatchRegex(@"^(.*) \| Thorkild-Kristensen$")
                .Groups[1].Value;

            return AddressLocation.Parse(titleText);
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var details = GetDetailNodes(document);

            var priceHtml = details["kontant"]
                .FirstChild
                .DecodedInnerHtml();

            return double.Parse(priceHtml.Replace(".", ""));
        }
        public async Task<EnergyTypes> GetEnergyType(HtmlDocument document)
        {
            var details = GetDetailNodes(document);

            var energyType = details["energimærke"]
                .FirstChild
                .DecodedInnerHtml()
                .Trim();

            if (!Enum.TryParse<EnergyTypes>(energyType, out var result))
                result = EnergyTypes.Unknown;

            return result;
        }

        public async Task<Area> GetGroundArea(HtmlDocument document)
        {
            var details = GetDetailNodes(document);

            var areaHtml = details["grund"]
                .FirstChild
                .DecodedInnerHtml();

            return new Area(double.Parse(areaHtml.Replace(".", "")), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var details = GetDetailNodes(document);

            var areaHtml = details["bolig"]
                .FirstChild
                .DecodedInnerHtml();

            return new Area(double.Parse(areaHtml.Replace(".", "")), AreaUnits.SquareMeter);
        }

        public async Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document)
        {
            return document.DocumentNode
                .SelectNodes("//li[@class='inner-wrapper']/img")
                .Select(x => x.GetAttributeValue("data-original", null))
                .Where(x => x != null)
                .Select(x => new HouseImage(new Uri(x)));
        }
    }
}
