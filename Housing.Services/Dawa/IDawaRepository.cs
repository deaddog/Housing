using Housing.Models;
using System.Threading.Tasks;

namespace Housing.Services.Dawa
{
    public interface IDawaRepository
    {
        Task<DawaAddress[]> SearchForHouse(AddressLocation address);
    }
}
