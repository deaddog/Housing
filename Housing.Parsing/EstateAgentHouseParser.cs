using Housing.Models;
using Housing.Scraping;
using Housing.Services.Boligsiden;
using Housing.Services.Dawa;
using Housing.Services.eTilbudsavis;
using Housing.Services.TjekDitNet;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Housing.Parsing
{
    public class EstateAgentHouseParser : IHouseParser
    {
        private static bool TryGetDomain(IEstateAgentHouseParser parser, out string domain)
        {
            domain = parser.SupportedDomain;
            var m = Regex.Match(domain, @"^(https?://)?(www\.)?(?<dom>.*[^ ]$)", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                domain = m.Groups["dom"].Value;
                return true;
            }
            else
            {
                domain = null;
                return false;
            }
        }

        private readonly IEstateAgentHouseParser _parser;
        private readonly string _domain;

        private readonly IBoligsidenRepository _boligsiden;
        private readonly IeTilbudsavisRepository _eTilbudsavis;
        private readonly IDawaRepository _dawa;
        private readonly ITjekDitNetRepository _tjekditnet;

        public delegate EstateAgentHouseParser Factory(IEstateAgentHouseParser parser);

        public EstateAgentHouseParser(IEstateAgentHouseParser parser, IBoligsidenRepository boligSiden, IeTilbudsavisRepository eTilbudsavis, IDawaRepository dawa, ITjekDitNetRepository tjekditnet)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            if (!TryGetDomain(parser, out _domain))
                throw new ArgumentException("Malformed domain name.");

            _boligsiden = boligSiden ?? throw new ArgumentNullException(nameof(boligSiden));
            _eTilbudsavis = eTilbudsavis ?? throw new ArgumentNullException(nameof(eTilbudsavis));
            _dawa = dawa ?? throw new ArgumentNullException(nameof(dawa));
            _tjekditnet = tjekditnet ?? throw new ArgumentNullException(nameof(tjekditnet));
        }

        public bool CanGetHouse(Uri houseUri)
        {
            return Regex.IsMatch(houseUri.AbsoluteUri, $@"^https?://(www\.)?{_domain.Replace(".", @"\.")}", RegexOptions.IgnoreCase);
        }
        public async Task<House> GetHouse(Uri houseUri)
        {
            var document = await houseUri.GetHtmlAsync().ConfigureAwait(false);

            var address = await _parser.GetAddress(document);
            var houseArea = await _parser.GetHouseArea(document);
            var groundArea = await _parser.GetGroundArea(document);
            var price = await _parser.GetPrice(document);
            var energyType = await _parser.GetEnergyType(document);

            var images = (await _parser.GetImages(document)).Distinct().ToArray();

            var boligSidenHouse = await _boligsiden.GetHouseDetailsAsync(address);
            var dawaHouse = (await _dawa.SearchForHouse(address)).FirstOrDefault();

            var shops = await _eTilbudsavis.GetShops(boligSidenHouse.Location, new string[0]);
            var internetOptions = dawaHouse != null ? await _tjekditnet.GetHouseInternetspeeds(dawaHouse.AddressId, dawaHouse.PostalCode) : new InternetOption[0];

            var today = DateTime.Today;

            return new House(
                houseUri,
                address,
                groundArea,
                houseArea,
                energyType,
                today.Subtract(boligSidenHouse.SalesPeriod),
                today.Subtract(boligSidenHouse.SalesPeriodTotal ?? boligSidenHouse.SalesPeriod),
                images,
                internetOptions,
                shops,
                new CategoryRating[0]
                );
        }
    }
}
