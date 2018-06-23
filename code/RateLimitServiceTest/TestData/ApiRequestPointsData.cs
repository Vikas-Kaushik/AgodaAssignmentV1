using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace RateLimitServiceTest.TestData
{
    public class ApiRequestPointsData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            uint max = 5;
            ushort slot = 5;
            ushort block = 0;

            // ToDo Carefull think of more test cases and add here
            // Note: The timeLine numbers need to be always in non-decreasing order
            
            // All requests at 10 seconds
            yield return new object[] { max, slot, block, 10, new uint[] { 10, 10, 10, 10, 10 } };

            yield return new object[] { max, slot, block, 14, new uint[] { 11, 11, 12, 12, 13 } };
            yield return new object[] { max, slot, block, 18, new uint[] { 14, 14, 14, 14, 14 } };
            yield return new object[] { max, slot, block, 19, new uint[] { 14, 14, 14, 15, 15 } };
        }
    }
}
