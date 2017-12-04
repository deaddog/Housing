using Housing.Models;
using Housing.Scraping;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Housing.Parsing.EstateAgentParsers
{
    class HomeEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "home.dk";

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var adr = document.DocumentNode.SelectSingleNode("//h2/span[@itemprop='streetAddress']").DecodedInnerHtml().Trim();
            var postal = int.Parse(document.DocumentNode.SelectSingleNode("//h2/span[@itemprop='postalCode']").DecodedInnerHtml().Trim());
            var city = document.DocumentNode.SelectSingleNode("//h2/span[@itemprop='addressLocality']").DecodedInnerHtml().Trim();

            return new AddressLocation(adr, postal, city);
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var priceItem = document.DocumentNode.SelectNodes("//ul/li/span[@class='info-property']").FirstOrDefault(x => x.InnerHtml.Trim() == "Kontantpris").ParentNode;
            var valueItem = priceItem.SelectSingleNode("./span[2]");

            var priceHtml = valueItem.InnerText.DecodeHtml().Trim();
            var m = Regex.Match(priceHtml, @"^(\d+(\.\d+)*) *kr\.$");

            return int.Parse(m.Groups[1].Value.Replace(".", ""));
        }
        public async Task<EnergyTypes> GetEnergyType(HtmlDocument document)
        {
            var areaItem = document.DocumentNode.SelectNodes("//ul/li/span[@class='info-property']").FirstOrDefault(x => x.InnerHtml.Trim() == "Energim&#230;rke").ParentNode;
            var valueItem = areaItem.SelectSingleNode("./span[2]");

            var energyType = valueItem.DecodedInnerText().Trim();

            if (!Enum.TryParse<EnergyTypes>(energyType, out var result))
                result = EnergyTypes.Unknown;

            return result;
        }

        public async Task<Area> GetGroundArea(HtmlDocument document)
        {
            var areaItem = document.DocumentNode.SelectNodes("//ul/li/span[@class='info-property']").FirstOrDefault(x => x.InnerHtml.Trim() == "Grundareal").ParentNode;
            var valueItem = areaItem.SelectSingleNode("./span[2]");

            var areaHtml = valueItem.InnerText.DecodeHtml().Trim();
            var m = Regex.Match(areaHtml, @"^(\d+(\.\d+)?) *m2$");

            return new Area(int.Parse(m.Groups[1].Value.Replace(".", "")), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var areaItem = document.DocumentNode.SelectNodes("//ul/li/span[@class='info-property']").FirstOrDefault(x => x.InnerHtml.Trim() == "Boligareal").ParentNode;
            var valueItem = areaItem.SelectSingleNode("./span[2]");

            var areaHtml = valueItem.InnerText.DecodeHtml().Trim();
            var m = Regex.Match(areaHtml, @"^(\d+(\.\d+)?) *m2$");

            return new Area(int.Parse(m.Groups[1].Value.Replace(".", "")), AreaUnits.SquareMeter);
        }

        public async Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document)
        {
            return document.DocumentNode.SelectNodes("//div[@class='gallery-view']//img[@data-sizes='auto']")?
                .Select(x => x.GetAttributeValue("data-src", null))
                .Where(x => x != null)
                .Select(x => new HouseImage(new Uri(x.Substring(0, x.LastIndexOf('&')))));
        }
    }
}
