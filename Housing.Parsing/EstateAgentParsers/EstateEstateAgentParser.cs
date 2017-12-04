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
    class EstateEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "estate.dk";

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var addressH1 = document.DocumentNode
                .SelectSingleNode("//div[@class='information']//h1")
                .InnerHtml
                .Split(new string[] { "<br>" }, StringSplitOptions.None)
                .Select(x => x.DecodeHtml().Trim())
                .ToArray();

            var adr = string.Join(", ", addressH1.Take(addressH1.Length - 1));
            return AddressLocation.Parse(adr, addressH1[addressH1.Length - 1]);
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var priceHtml = document.DocumentNode.SelectNodes("//table/tr/th").First(x => x.InnerHtml == "Kontantpris").ParentNode.SelectSingleNode("./td").InnerHtml;

            return double.Parse(priceHtml.Replace(".", ""));
        }
        public async Task<EnergyTypes> GetEnergyType(HtmlDocument document)
        {
            var energyType = document
                .DocumentNode
                .SelectSingleNode("//li[@title='Energim&#230;rke']/div")?
                .NextSibling
                .InnerHtml
                .Trim();

            if (!Enum.TryParse<EnergyTypes>(energyType, out var result))
                result = EnergyTypes.Unknown;

            return result;
        }

        public async Task<Area> GetGroundArea(HtmlDocument document)
        {
            var areaHtml = document
                .DocumentNode
                .SelectSingleNode("//li[@title='Grundstr.']/div")?
                .NextSibling
                .DecodedInnerHtml()
                .Trim();

            var m = Regex.Match(areaHtml, @"(\d+) *m²");
            return new Area(double.Parse(m.Groups[1].Value), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var areaHtml = document
                .DocumentNode
                .SelectSingleNode("//li[@title='Boligareal']/div")?
                .NextSibling
                .DecodedInnerHtml()
                .Trim();

            var m = Regex.Match(areaHtml, @"(\d+) *m²");
            return new Area(double.Parse(m.Groups[1].Value), AreaUnits.SquareMeter);
        }

        public async Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document)
        {
            return document.DocumentNode.SelectNodes("//meta[@property='og:image']")?
                .Select(x => x.GetAttributeValue("content", null))
                .Where(x => x != null)
                .Select(x => new HouseImage(new Uri(x)));
        }
    }
}
