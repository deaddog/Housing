using Housing.Models;
using Housing.Scraping;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Housing.Parsing.EstateAgentParsers
{
    class NyBoligEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "nybolig.dk";

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var address = document.DocumentNode
                .SelectSingleNode("//strong[@class='case-info__property__info__main__title__address']")
                .DecodedInnerHtml()
                .Trim();

            var cityStr = document.DocumentNode
                .SelectSingleNode("//em[@class='case-info__property__info__main__title__location']")
                .DecodedInnerHtml()
                .Trim();

            return AddressLocation.Parse(address, cityStr);
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var priceHtml = document.DocumentNode.SelectSingleNode("//span[@class='case-info__property__info__text__price']").DecodedInnerHtml();

            return int.Parse(priceHtml.Replace(".", ""));
        }
        public async Task<EnergyTypes> GetEnergyType(HtmlDocument document)
        {
            var energyType = document.DocumentNode
                .SelectSingleNode("//li[@class='case-facts__major__item fact-icon fact-icon--energyclassification']/strong")?
                .DecodedInnerHtml()
                .Trim();

            if (!Enum.TryParse<EnergyTypes>(energyType, out var result))
                result = EnergyTypes.Unknown;

            return result;
        }

        public async Task<Area> GetGroundArea(HtmlDocument document)
        {
            var m = document.DocumentNode
                .SelectSingleNode("//li[@class='case-facts__major__item fact-icon fact-icon--propertysize']/strong")
                .DecodedInnerHtml()
                .Trim()
                .MatchRegex(@"(\d+) *m²");

            return new Area(int.Parse(m.Groups[1].Value), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var m = document.DocumentNode
                .SelectSingleNode("//li[@class='case-facts__major__item fact-icon fact-icon--livingspace']/strong")
                .DecodedInnerHtml()
                .Trim().MatchRegex(@"(\d+) *m²");

            return new Area(int.Parse(m.Groups[1].Value), AreaUnits.SquareMeter);
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
