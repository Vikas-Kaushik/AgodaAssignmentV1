using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hotels.API.Models;

namespace Hotels.API.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        public IList<HotelDto> Hotels { get; set; }

        public HotelRepository()
        {
            Hotels = File.ReadAllLines("Repositories\\hoteldb.csv")
                .Skip(1)
                .Select(line => HotelDto.FromCsvLine(line))
                .ToList();
        }

        public IEnumerable<HotelDto> GetAll()
        {
            return Hotels;
        }
    }
}
