using System;
using RateLimitServices.Utilities;

namespace RateLimitServices
{
    public class RateLimitService : IRateLimitService
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

        public RateLimitService(
            uint maxRequestsInSlotSpan,
            ushort slotSpanInSeconds,
            ushort timeSpanToKeepBlockedInSeconds)
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

        // ===================================================================
        // Here's the Core Logic:
        public bool IsAllowed(ushort presentIndex)
        {
            if (_pastIndex > presentIndex)
            {
                _totalRequestsInSlotSpan -= (_requestCounts[presentIndex] - 1);
                _requestCounts[presentIndex] = 1;
            }
            else
            {
                _totalRequestsInSlotSpan += 1;
                _requestCounts[presentIndex] += 1;
            }

            _pastIndex = presentIndex;

            if (_totalRequestsInSlotSpan > _maxRequestsInSlotSpan)
            {
                _totalRequestsInSlotSpan -= 1;
                return false;
            }

            return true;
        }

        // ===================================================================
        public bool IsLimitedApi => _IsLimitedApi();

        public bool _IsLimitedApi()
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

        public bool IsThresholdCrossed(uint now)
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
                ++_totalRequestsInSlotSpan;
            }
            else
            {                
                ++_totalRequestsInSlotSpan;
            }

            _pastIndex = presentIndex;
            _timeOfLastApiCall = now;

            if(_totalRequestsInSlotSpan > _maxRequestsInSlotSpan)
            {
                --_totalRequestsInSlotSpan;
                _lastTimeWhenThresholdCrossed = now;
                return true;
            }

            ++_requestCounts[presentIndex];

            return false;
        }

        public bool ShouldBeKeptBlockedFor(uint now)
        {
            var timSpanSinceThreshHoldCrossedLastTime = now - _lastTimeWhenThresholdCrossed;

            return timSpanSinceThreshHoldCrossedLastTime <= _timeSpanToKeepBlockedInSeconds;
        }
    }
}
