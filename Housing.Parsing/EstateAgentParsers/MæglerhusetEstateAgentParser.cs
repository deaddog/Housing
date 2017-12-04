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
    class MæglerhusetEstateAgentParser : IEstateAgentHouseParser
    {
        public string SupportedDomain => "xn--mglerhuset-d6a.dk";

        private Dictionary<string, HtmlNode> GetDetailNodes(HtmlDocument document)
        {
            return document.DocumentNode
                  .SelectNodes("//div[@class='case-facts']/ul/li")
                  .Select(x =>
                  {
                      var spans = x.SelectNodes("./span");
                      var key = spans[0].DecodedInnerHtml().ToLower();
                      var value = spans[1];

                      return (Key: key, Node: value);
                  })
                  .ToDictionary(x => x.Key, x => x.Node);
        }

        public async Task<AddressLocation> GetAddress(HtmlDocument document)
        {
            var adrString = document.DocumentNode
                .SelectSingleNode("//section[@class='title-band']/div/div/h1")
                .DecodedInnerHtml()
                .Trim();

            adrString = Regex.Replace(adrString, "  +", " ");
            return AddressLocation.Parse(adrString);
        }
        public async Task<double> GetPrice(HtmlDocument document)
        {
            var details = GetDetailNodes(document);

            var match = details["pris"]
                .FirstChild
                .DecodedInnerHtml()
                .MatchRegex(@"(\d+(\.\d+)*) kr.");

            return double.Parse(match.Groups[1].Value.Replace(".", ""));
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

            var match = details["grundareal"]
                .FirstChild
                .DecodedInnerHtml()
                .MatchRegex(@"(\d+(\.\d+)*) m");

            return new Area(double.Parse(match.Groups[1].Value.Replace(".", "")), AreaUnits.SquareMeter);
        }
        public async Task<Area> GetHouseArea(HtmlDocument document)
        {
            var details = GetDetailNodes(document);

            var match = details["boligareal"]
                .FirstChild
                .DecodedInnerHtml()
                .MatchRegex(@"(\d+) m");

            return new Area(double.Parse(match.Groups[1].Value), AreaUnits.SquareMeter);
        }

        public async Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document)
        {
            return document.DocumentNode
                .SelectNodes("//div[@id='fullscreen-window']/div[@id='fullscreen-window-container']/div[@class='slide']")
                .Select(x =>
                {
                    var style = x.GetAttributeValue("style", null);
                    if (style == null)
                        return null;

                    var match = style.MatchRegex(@"^background-image: url\((.*)\)$");
                    if (match.Success)
                        return new Uri(match.Groups[1].Value);
                    else
                        return null;
                })
                .Where(x => x != null)
                .Select(x => new HouseImage(x))
                .ToList();
        }
    }
}
