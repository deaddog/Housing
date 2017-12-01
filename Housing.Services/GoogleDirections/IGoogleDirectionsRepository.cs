using Housing.Models;
using System.Threading.Tasks;

namespace Housing.Services.GoogleDirections
{
    public interface IGoogleDirectionsRepository
    {
        Task<Distance> GetDistance(string origin, string destination);
    }
}
