using System.Collections.Generic;
using Hotels.API.Models;

namespace Hotels.API.Repositories
{
    public interface IHotelRepository
    {
        IEnumerable<HotelDto> GetAll();
    }
}
