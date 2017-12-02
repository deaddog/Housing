using Flurl;
using Housing.Models;
using Housing.Scraping;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Housing.Services.TjekDitNet
{
    public class TjekDitNetRepository : ITjekDitNetRepository
    {
        public async Task<InternetOption[]> GetHouseInternetspeeds(Guid dawaAddressId, int postalCode)
        {
            var html = await $"https://tjekditnet.dk/pls/wopdprod/tdnutils.find_udbydere_html"
                .SetQueryParam("i_adgadr_id", dawaAddressId)
                .SetQueryParam("i_postnummer", postalCode)
                .GetHtmlAsync()
                .ConfigureAwait(false);

            var optionNodes = html.DocumentNode.SelectNodes("//div[@class='find-isp']/div")
                .SkipWhile(x => x.GetAttributeValue("class", "") == "row find-isp-header")
                .TakeWhile(x => x.GetAttributeValue("class", "") == "find-isp-content level-1")
                .ToList();

            var speeds = new List<InternetOption>();

            foreach (var option in optionNodes)
            {
                var speedNode = option.SelectSingleNode("./div/div[1]");
                var typeNode = option.SelectSingleNode("./div/div[2]");

                if (TryGetSpeeds(speedNode, out var downloadSpeed, out var uploadSpeed))
                {
                    var connectionType = GetConnectionType(typeNode);
                    speeds.Add(new InternetOption(downloadSpeed, uploadSpeed, connectionType));
                }
            }

            return speeds.ToArray();
        }

        private bool TryGetSpeeds(HtmlNode node, out double downloadSpeed, out double uploadSpeed)
        {
            var speedText = node.InnerText.DecodeHtml().Trim().Trim('*').Trim();

            if (speedText == "Dækning")
            {
                downloadSpeed = default(double);
                uploadSpeed = default(double);
                return false;
            }

            Match match = Regex.Match(speedText, @"(?<download>\d+) */ *(?<upload>\d+) *Mbit/s");
            if (match.Success)
            {
                downloadSpeed = double.Parse(match.Groups["download"].Value);
                uploadSpeed = double.Parse(match.Groups["upload"].Value);

                return true;
            }
            match = Regex.Match(speedText, @"(?<lower>\d+) *- *(?<upper>\d+) *Mbit/s");
            if (match.Success)
            {
                downloadSpeed = double.Parse(match.Groups["upper"].Value);
                uploadSpeed = double.Parse(match.Groups["upper"].Value);

                return true;
            }

            downloadSpeed = default(double);
            uploadSpeed = default(double);
            return false;
        }
        private InternetConnectionTypes GetConnectionType(HtmlNode node)
        {
            var typeText = node.InnerText.DecodeHtml().Trim().Trim('*').Trim();

            switch (typeText)
            {
                case "Kobbernet (xDSL)": return InternetConnectionTypes.DSL;
                case "Mobilt bredbånd": return InternetConnectionTypes.Mobile;
                case "Fast-trådløst": return InternetConnectionTypes.FixedMobile;
                case "Kabel-TV": return InternetConnectionTypes.Cable;
                case "Fiber": return InternetConnectionTypes.Optic;

                default: return InternetConnectionTypes.Unknown;
            }
        }
    }
}
