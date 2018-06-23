using System;
using System.Collections.Generic;
using RateLimitServices;
using RateLimitServices.Utilities;
using RateLimitServiceTest.TestData;
using Xunit;

namespace RateLimitServiceTest
{
    public class RateLimitServiceShould
    {
        [Theory]
        [ApiRequestPointsData]
        public void WorkAsExpected(
            uint max, ushort slot, ushort block, ushort now, params uint[] timeLine)
        {
            IRateLimitCache cache = new RateLimitCacheForCityApi(max, slot, block);

            foreach (var time in timeLine)
            {
                // Threshold not crossed for given time line
                Assert.False(RateLimitService.IsThresholdCrossed(time, cache));
            }

            // Threshold crossed now
            Assert.True(RateLimitService.IsThresholdCrossed(now, cache));
        }
    }
}