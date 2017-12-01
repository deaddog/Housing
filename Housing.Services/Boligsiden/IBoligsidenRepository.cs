using Housing.Models;
using System.Threading.Tasks;

namespace Housing.Services.Boligsiden
{
    public interface IBoligsidenRepository
    {
        Task<BoligsidenHouseDetails> GetHouseDetailsAsync(AddressLocation address);
    }
}
