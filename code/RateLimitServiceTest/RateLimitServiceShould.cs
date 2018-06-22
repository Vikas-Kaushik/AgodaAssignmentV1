using System;
using System.Collections.Generic;
using RateLimitServices;
using Xunit;

namespace RateLimitServiceTest
{
    public class RateLimitServiceShould
    {
        [Fact]
        public void WorkAsExpected()
        {
            var rateLimiter = new RateLimitService(10, 10, 5);

            // 10 requests received at 0
            IList<ushort> inputs = new List<ushort>(new ushort[10]);

            foreach (var i in inputs)
            {
                Assert.True(rateLimiter.IsAllowed(i));
            }

            // 11 request should fail
            Assert.False(rateLimiter.IsAllowed(0));

            //===========================
            rateLimiter = new RateLimitService(10, 10, 5);

            // 10 requests received at 0
            inputs = new List<ushort>() { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };

            foreach (var i in inputs)
            {
                Assert.True(rateLimiter.IsAllowed(i));
            }

            // 11 request should fail
            Assert.False(rateLimiter.IsAllowed(9));

            //===========================
            rateLimiter = new RateLimitService(10, 10, 5);

            // 10 requests received at 0
            inputs = new List<ushort>() { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };

            foreach (var i in inputs)
            {
                Assert.True(rateLimiter.IsAllowed(i));
            }

            // 11 request should fail
            Assert.False(rateLimiter.IsAllowed(0));

            //===========================
            rateLimiter = new RateLimitService(10, 10, 5);

            // 10 requests received at 0
            inputs = new List<ushort>() { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };

            foreach (var i in inputs)
            {
                Assert.True(rateLimiter.IsAllowed(i));
            }

            // 11 request should fail
            Assert.False(rateLimiter.IsAllowed(5));
        }
    }
}
