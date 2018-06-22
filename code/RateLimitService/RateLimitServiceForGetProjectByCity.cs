using System;
using RateLimitServices.Utilities;

namespace RateLimitServices
{
    public class RateLimitServiceForGetProjectByCity : IRateLimitServiceForGetProjectByCity
    {
        private readonly uint _maxRequestsInSlotSpan;
        private readonly ushort _slotSpanInSeconds;
        private readonly ushort _timeSpanToKeepBlockedInSeconds;
        private uint _totalRequestsInSlotSpan;
        private ushort _pastIndex;
        private uint[] _requestCounts;
        private uint _timeOfLastApiCall;
        private uint _lastTimeWhenThresholdCrossed;        
        private readonly object thisLock = new object();

        public RateLimitServiceForGetProjectByCity(
            uint maxRequestsInSlotSpan = 50,
            ushort slotSpanInSeconds = 10,
            ushort timeSpanToKeepBlockedInSeconds = 5)
        {
            _maxRequestsInSlotSpan = maxRequestsInSlotSpan;
            _slotSpanInSeconds = slotSpanInSeconds;
            _timeSpanToKeepBlockedInSeconds = timeSpanToKeepBlockedInSeconds;

            _totalRequestsInSlotSpan = 0;
            _timeOfLastApiCall = 0;
            _pastIndex = 0;

            _requestCounts = new uint[_slotSpanInSeconds];
            _requestCounts.Initialize();
        }

        public bool IsLimited => _IsLimitedApiGetHotelsByCity();

        private bool _IsLimitedApiGetHotelsByCity()
        {
            // get now without milliseconds
            var now = UnixEpoch.Now;

            lock (thisLock)
            {
                // If API has been blocked in last 5 seconds: return True 
                if (ShouldBeKeptBlockedFor(now)) return true;

                // If API has been already called, 10 times in last 10 seconds: return True
                if (IsThresholdCrossed(now)) return true;
            }

            // Don't limit the API call               
            return false;
        }

        internal bool IsThresholdCrossed(uint now)
        {            
            ushort presentIndex = Convert.ToUInt16(now % _slotSpanInSeconds);

            var secondsSinceLastRequest = now - _timeOfLastApiCall;

            if (secondsSinceLastRequest > _slotSpanInSeconds)
            {
                _totalRequestsInSlotSpan = 0;
                _requestCounts.Initialize();
                _pastIndex = 0;
            }
            
            if(presentIndex > _pastIndex)
            {
                _totalRequestsInSlotSpan -= _requestCounts[presentIndex];
                _requestCounts[presentIndex] = 1;
            }
            else
            {              
                ++_requestCounts[presentIndex];
            }

            ++_totalRequestsInSlotSpan;
            _pastIndex = presentIndex;
            _timeOfLastApiCall = now;

            if(_totalRequestsInSlotSpan > _maxRequestsInSlotSpan)
            {
                --_totalRequestsInSlotSpan;                
                --_requestCounts[presentIndex];
                _lastTimeWhenThresholdCrossed = now;
                return true;
            }            

            return false;
        }

        internal bool ShouldBeKeptBlockedFor(uint now)
        {
            var timSpanSinceThreshHoldCrossedLastTime = now - _lastTimeWhenThresholdCrossed;

            return timSpanSinceThreshHoldCrossedLastTime <= _timeSpanToKeepBlockedInSeconds;
        }
    }
}
