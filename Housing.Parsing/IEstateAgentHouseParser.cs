using Housing.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Housing.Parsing
{
    public interface IEstateAgentHouseParser
    {
        string SupportedDomain { get; }

        Task<AddressLocation> GetAddress(HtmlDocument document);

        Task<Area> GetHouseArea(HtmlDocument document);
        Task<Area> GetGroundArea(HtmlDocument document);

        Task<double> GetPrice(HtmlDocument document);
        Task<EnergyTypes> GetEnergyType(HtmlDocument document);

        Task<IEnumerable<HouseImage>> GetImages(HtmlDocument document);
    }
}
