using System;

namespace Hotels.API.Models
{
    public class HotelDto
    {
        public long Id { get; set; }
        public string City { get; set; }
        public string Room { get; set; }
        public double Price { get; set; }

        /// <summary>
        /// Reads a csv file line to HotelDto instance
        /// </summary>
        /// <param name="csvLine">comma separated values in order: CITY,HOTELID,ROOM,PRICE</param>
        /// <returns>HotelDto instance</returns>
        public static HotelDto FromCsvLine(string csvLine)
        {
            string[] values = csvLine.Split(',');

            return new HotelDto()
            {
                City = values[0],
                Id = Convert.ToInt64(values[1]),
                Room = values[2],
                Price = Convert.ToDouble(values[3])
            };
        }
    }
}
