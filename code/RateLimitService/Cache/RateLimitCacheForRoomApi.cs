using System;
using Microsoft.Extensions.Configuration;

namespace RateLimitServices
{
    public class RateLimitCacheForRoomApi : IRateLimitCache
    {
        public RateLimitCacheForRoomApi(IConfiguration configuration = null)
        {
            // Set default values
            MaxRequestsInSlotSpan = 50;
            SlotSpanInSeconds = 10;
            TimeSpanToKeepBlockedInSeconds = 0;

            TimeOfLastApiCall = 0;
            TotalRequestsInSlotSpan = 0;
            PastIndex = 0;
            LastTimeWhenThresholdCrossed = 0;

            if (!(configuration is null))
            {
                // Read configuraton values
                string maxRequestsStr = configuration["RateLimitService:GetHotelsByRoom:maxNumberOfRequests"];
                string slotSpanStr = configuration["RateLimitService:GetHotelsByRoom:slotSpan"];
                string timeToBlockStr = configuration["RateLimitService:GetHotelsByRoom:timeToBlock"];

                // Update readonly fields, Only if configuration has valid values
                try
                {
                    if (!string.IsNullOrWhiteSpace(maxRequestsStr))
                        MaxRequestsInSlotSpan = Convert.ToUInt32(maxRequestsStr);

                    if (!string.IsNullOrWhiteSpace(slotSpanStr))
                        SlotSpanInSeconds = Convert.ToUInt16(slotSpanStr);

                    if (!string.IsNullOrWhiteSpace(maxRequestsStr))
                        TimeSpanToKeepBlockedInSeconds = Convert.ToUInt16(timeToBlockStr);
                }
                catch
                {
                    // log: "Issue while converting configurations values for GetHotelsForCityApi.";
                    // Continue with default values 
                }
            }

            RequestCounts = new uint[SlotSpanInSeconds];
            RequestCounts.Initialize();
        }

        // Read-Only properties
        public uint SlotSpanInSeconds { get; }
        public uint[] RequestCounts { get; }
        public uint MaxRequestsInSlotSpan { get; }
        public uint TimeSpanToKeepBlockedInSeconds { get; }

        // Mutable properties
        public uint TimeOfLastApiCall { get; set; }
        public uint TotalRequestsInSlotSpan { get; set; }
        public ushort PastIndex { get; set; }
        public uint LastTimeWhenThresholdCrossed { get; set; }
    }
}
