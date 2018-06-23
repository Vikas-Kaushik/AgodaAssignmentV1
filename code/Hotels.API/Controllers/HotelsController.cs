using System;
using System.Linq;
using Hotels.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimitServices;

namespace Hotels.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IRateLimitService _rateLimitService;
        private readonly ILogger<HotelsController> _logger;
        private readonly IHotelRepository _hotelRepository;

        public HotelsController(
            IHotelRepository hotelRepository,
            IRateLimitService rateLimitService,
            ILogger<HotelsController> logger)
        {
            _rateLimitService = rateLimitService ?? throw new ArgumentNullException(nameof(rateLimitService));

            _hotelRepository = hotelRepository ?? throw new ArgumentException(nameof(hotelRepository));

            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        // Get /api/hotels/city/bangkok
        [HttpGet("city/{city}", Name = "GetHotelsByCity")]
        public IActionResult GetHotelsByCity(string city)
        {
            if (_rateLimitService.IsLimited(RateLimitedApis.GetHotelsByCity))
            {
                // TooManyRequests
                return StatusCode(429, "Please try after some time.");
            }

            var cityHotels = _hotelRepository.GetAll()
                .Where(hotel => hotel.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(cityHotels);
        }

        // Get /api/hotels/room/deluxe
        [HttpGet("room/{roomType}", Name = "GetHotelsByRoom")]
        public IActionResult GetHotelsByRoom(string roomType)
        {
            if (_rateLimitService.IsLimited(RateLimitedApis.GetHotelsByRoom))
            {
                // TooManyRequests
                return StatusCode(429, "Please try after some time.");
            }

            var roomHotels = _hotelRepository.GetAll()
                .Where(hotel => hotel.Room.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(roomHotels);
        }
    }
}
