using Housing.Models;
using System;
using System.Threading.Tasks;

namespace Housing.Parsing
{
    public interface IHouseParser
    {
        bool CanGetHouse(Uri houseUri);
        Task<House> GetHouse(Uri houseUri);
    }
}
