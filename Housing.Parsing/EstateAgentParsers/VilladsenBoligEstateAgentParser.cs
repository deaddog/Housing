using Housing.Models;
using Housing.Scraping;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Housing.Parsing.EstateAgentParsers
{
    class VilladsenBoligEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "villadsenbolig.dk";

        private Dictionary<string, HtmlNode> GetDetailNodes(HtmlDocument document)
        {
            return document.DocumentNode
                  .SelectNodes("//div[@class='single-sidebar-description']/table/tr")
                  .Select(x =>
                  {
                      var spans = x.SelectNodes("./td");
                      if (spans.Count != 2)
                          return (Key: null, Node: null);

                      var key = spans[0].DecodedInnerText().ToLower().Trim().TrimEnd('.', ':');
                      var value = spans[1];

                      return (Key: key, Node: value);
                  })
                  .Where(x => x.Key != null && x.Node != null)
                  .ToDictionary(x => x.Key, x => x.Node);
        }

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var adr1 = document.DocumentNode.SelectSingleNode("//div[@class='single-sidebar-title']").DecodedInnerHtml().Trim();
            var adr2 = document.DocumentNode.SelectSingleNode("//div[@class='single-sidebar-subtitle']").DecodedInnerHtml().Trim();

            var sb = new StringBuilder(adr1 + ", " + adr2);
            for (int i = 0; i < sb.Length; i++)
                if (char.IsWhiteSpace(sb[i]))
                    sb[i] = ' ';
            
            return AddressLocation.Parse(sb.ToString());
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var priceHtml = GetDetailNodes(document)["pris"].DecodedInnerText();
            return double.Parse(priceHtml.Replace(".", ""));
        }
        public async Task<EnergyTypes> GetEnergyType(HtmlDocument document)
        {
            var energyType = GetDetailNodes(document)["energimærkning"].DecodedInnerText();

            if (!Enum.TryParse<EnergyTypes>(energyType, out var result))
                result = EnergyTypes.Unknown;

            return result;
        }

        public async Task<Area> GetGroundArea(HtmlDocument document)
        {
            var groundArea = GetDetailNodes(document)["grundareal"].DecodedInnerText().Trim('²', 'm');
            return new Area(double.Parse(groundArea.Replace(".", "")), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var houseArea = GetDetailNodes(document)["boligareal"].DecodedInnerText().Trim('²', 'm');
            return new Area(double.Parse(houseArea.Replace(".", "")), AreaUnits.SquareMeter);
        }

        public async Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document)
        {
            return document.DocumentNode
                .SelectNodes("//div[@class='single-gallery-image single-gallery-image-gallery']")
                .Select(x => x.ParentNode.SelectSingleNode("./a"))
                .Select(node => node.GetAttributeValue("href", null))
                .Where(x => x != null)
                .Select(x => new HouseImage(new Uri(x.DecodeHtml())));
        }
    }
}
