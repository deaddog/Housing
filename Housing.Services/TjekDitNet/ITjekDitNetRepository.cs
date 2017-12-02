using Housing.Models;
using System;
using System.Threading.Tasks;

namespace Housing.Services.TjekDitNet
{
    public interface ITjekDitNetRepository
    {
        Task<InternetOption[]> GetHouseInternetspeeds(Guid dawaAddressId, int postalCode);
    }
}
