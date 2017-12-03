using Housing.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Housing.Parsing
{
    public interface IEstateAgentHouseParser
    {
        string SupportedDomain { get; }

        Task<AddressLocation> GetAddress(HtmlDocument doc);

        Task<Area> GetHouseArea(HtmlDocument doc);
        Task<Area> GetGroundArea(HtmlDocument doc);

        Task<double> GetPrice(HtmlDocument doc);
        Task<EnergyTypes> GetEnergyType(HtmlDocument doc);

        Task<IEnumerable<HouseImage>> GetImages(HtmlDocument doc);
    }
}
