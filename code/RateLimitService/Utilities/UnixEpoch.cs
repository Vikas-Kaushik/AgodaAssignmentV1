using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimitServices.Utilities
{
    public class UnixEpoch
    {
        public static DateTime EpochTime => new DateTime(1970, 1, 1);

        public static uint Now
        {
            get
            {
                var now = DateTime.UtcNow;

                var timeSinceEpoch = now.Subtract(EpochTime);

                return (uint)timeSinceEpoch.TotalSeconds;
            }
        }
    }
}
