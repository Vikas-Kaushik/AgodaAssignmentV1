using System;
using Microsoft.Extensions.Configuration;
using RateLimitServices.Utilities;

namespace RateLimitServices
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IRateLimitCache _rateLimitCacheForCityApi;
        private readonly IRateLimitCache _rateLimitCacheForRoomApi;
        private readonly object _cityApiLock = new object();
        private readonly object _roomApiLock = new object();

        public RateLimitService(IConfiguration configuration)
        {
            _rateLimitCacheForCityApi = new RateLimitCacheForCityApi(configuration);
            _rateLimitCacheForRoomApi = new RateLimitCacheForRoomApi(configuration);
        }

        public bool IsLimited(RateLimitedApis rateLimitedApi)
        {
            switch(rateLimitedApi)
            {
                case RateLimitedApis.GetHotelsByCity:
                    return IsLimitedApi(_rateLimitCacheForCityApi, _cityApiLock);

                case RateLimitedApis.GetHotelsByRoom:
                    return IsLimitedApi(_rateLimitCacheForRoomApi, _roomApiLock);

                default:
                    return true;
            }
        }

        public bool IsLimitedApi(IRateLimitCache cache, Object lockMe)
        {
            // get now without milliseconds
            var now = UnixEpoch.Now;

            lock (lockMe)
            {
                if (ShouldBeKeptBlockedFor(now, cache)) return true;

                // This will be executed only after timeToBlock seconds of last time threshold crossed.
                if (IsThresholdCrossed(now, cache)) return true;
            }

            // Don't limit the API call               
            return false;
        }

        /// <summary>
        /// Tells if threshold is crossed now with current state of cache
        /// 
        /// Following ideas are important for this algorithm:
        /// 1. Keep a circular queue (array) of request counts on indexes, where index represents
        ///    a time in the slot span window. The same queue will be overlapped for future requests,
        ///    size of the queue represents the time slot and the slot window can be begninning or ending
        ///    at any index.
        /// 2. Time always moves forward
        ///    Beginning of slot (i.e. past) will always be behind the slot ending (i.e. present)
        /// </summary>
        /// <param name="cache">
        /// cache.RequestCounts: Keeps request counts of calls in curentwindow
        /// cache.TotalRequestsInSlotSpan: Keeps track of total number of request counts in current window,
        /// current window is based on Now and Present Index
        /// </param>
        /// <returns></returns>
        public static bool IsThresholdCrossed(uint now, IRateLimitCache cache)
        {
            // present index in circular queue "RequestCounts"
            ushort presentIndex = Convert.ToUInt16(now % cache.SlotSpanInSeconds);

            if(now < cache.TimeOfLastApiCall)
            {
                // now is older than the time when last api was received.
                throw new InvalidOperationException($"Time Machine usage is not allowed.");
            }

            var secondsSinceLastRequest = now - cache.TimeOfLastApiCall;

            // If it's been more than slot span, then clear the stale values
            if (secondsSinceLastRequest > cache.SlotSpanInSeconds)
            {
                // following properties become obsololete, out of the slot window, so reset
                cache.TotalRequestsInSlotSpan = 0;
                cache.RequestCounts.Initialize();
                cache.PastIndex = 0;
            }

            // Fact: Time always moves forward and
            // Past is always behind present
            if (cache.PastIndex > presentIndex)
            {
                // If present is behind past, it's an overlap
                // i.e. the request count value on present index is from past cycle,
                // which means, it is not part of the current slot window
                // Also it was part of TotalRequestsInSlotSpan, which is not anymore.
                cache.TotalRequestsInSlotSpan -= cache.RequestCounts[presentIndex];

                // make this request as first request on presentIndex, a new slot window is entered.
                cache.RequestCounts[presentIndex] = 1;            
            }

            // Add a successful request in TotalRequestsInSlotSpan
            ++cache.TotalRequestsInSlotSpan;

            // present will become past 
            cache.PastIndex = presentIndex;
            cache.TimeOfLastApiCall = now;

            if(cache.TotalRequestsInSlotSpan > cache.MaxRequestsInSlotSpan)
            {
                // As it was not a successful request, remove it from TotalRequestsInSlotSpan
                --cache.TotalRequestsInSlotSpan;

                // Update LastTimeWhenThresholdCrossed, so that all APIs will be blocked for next timeToBlock seconds
                cache.LastTimeWhenThresholdCrossed = now;

                return true;
            }

            // Add a successful request at present index in RequestCounts
            ++cache.RequestCounts[presentIndex];

            return false;
        }

        public static bool ShouldBeKeptBlockedFor(uint now, IRateLimitCache cache)
        {
            var timSpanSinceThreshHoldCrossedLastTime = now - cache.LastTimeWhenThresholdCrossed;

            return timSpanSinceThreshHoldCrossedLastTime <= cache.TimeSpanToKeepBlockedInSeconds;
        }
    }
}
