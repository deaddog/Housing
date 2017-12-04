using Housing.Models;
using Housing.Scraping;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Housing.Parsing.EstateAgentParsers
{
    class EdcEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "edc.dk";

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var adrNode = document.DocumentNode.SelectSingleNode("//div[@class='name']/h1");

            var adr = adrNode.ChildNodes[0].DecodedInnerHtml().Trim().Trim(',').Trim();
            var codeCity = adrNode.ChildNodes[1].DecodedInnerHtml().Trim().Trim(',').Trim();

            return AddressLocation.Parse(adr, codeCity);
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var priceHtml = document.DocumentNode
                .SelectSingleNode("//h2[@class='price']")
                .DecodedInnerHtml();

            var match = Regex.Match(priceHtml, @"(\d+(\.\d+)*) kr\.");

            return double.Parse(match.Groups[1].Value.Replace(".", ""));
        }
        public async Task<EnergyTypes> GetEnergyType(HtmlDocument document)
        {
            var energyType = document.DocumentNode
                .SelectNodes("//div[@class='case-facts__fact cf case-facts__two-col']")
                .Where(x => x.SelectSingleNode("./p[@class='info']")?.DecodedInnerHtml() == "Energimærke")
                .Select(x => x.SelectSingleNode("./p[@class='value']")?.DecodedInnerText().Trim())
                .First();

            if (!Enum.TryParse<EnergyTypes>(energyType, out var result))
                result = EnergyTypes.Unknown;

            return result;
        }

        public async Task<Area> GetGroundArea(HtmlDocument document)
        {
            var areaHtml = document.DocumentNode
                .SelectNodes("//div[@class='case-facts__fact cf case-facts__two-col']")
                .Where(x => x.SelectSingleNode("./p[@class='info']")?.DecodedInnerHtml() == "Grundareal")
                .Select(x => x.SelectSingleNode("./p[@class='value']")?.DecodedInnerHtml())
                .First();

            var m = Regex.Match(areaHtml.Replace(".", ""), @"(\d+) *m²");
            return new Area(double.Parse(m.Groups[1].Value), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var areaHtml = document.DocumentNode
                .SelectNodes("//div[@class='case-facts__fact cf case-facts__two-col']")
                .Where(x => x.SelectSingleNode("./p[@class='info']")?.DecodedInnerHtml() == "Boligareal")
                .Select(x => x.SelectSingleNode("./p[@class='value']")?.DecodedInnerHtml())
                .First();

            var m = Regex.Match(areaHtml, @"(\d+) *m²");
            return new Area(double.Parse(m.Groups[1].Value), AreaUnits.SquareMeter);
        }

        public async Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document)
        {
            var imagesJson = document.DocumentNode
                .SelectSingleNode("//div[@data-js='json']").InnerHtml;

            var images = new List<HouseImage>();

            foreach (var img in JArray.Parse(imagesJson))
            {
                var type = GetImageType(img.Value<string>("Type").ToLower());
                var url = img.Value<string>("LargeImageUrl");

                if (!type.HasValue)
                    continue;

                images.Add(new HouseImage(new Uri("http:" + url), type.Value));
            }

            return images;
        }

        private ImageTypes? GetImageType(string imageType)
        {
            switch (imageType)
            {
                case "blueprint": return ImageTypes.Floorplan;
                case "picture": return ImageTypes.Unknown;
                default: return null;
            }
        }
    }
}
