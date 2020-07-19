using System;

namespace YourList.Internal
{
    internal static class Epoch
    {
        public static DateTimeOffset ToDateTimeOffset(long offsetSeconds)
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero) + TimeSpan.FromSeconds(offsetSeconds);
        }
    }
}