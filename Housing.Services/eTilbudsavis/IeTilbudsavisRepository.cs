using Housing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Housing.Services.eTilbudsavis
{
    public interface IeTilbudsavisRepository
    {
        Task<List<Dealer>> GetDealers();
        Task<List<Shop>> GetShops(GeoLocation location, IEnumerable<string> dealerIds);
    }
}
