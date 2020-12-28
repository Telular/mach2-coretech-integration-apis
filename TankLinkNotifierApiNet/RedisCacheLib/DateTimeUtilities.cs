using System;

namespace RedisCacheLib
{
    public enum EpochFormat
    {
        Seconds,
        Milliseconds
    }

    public static class DateTimeUtilities
    {
        public static long ToEpochTime(this DateTime dt, EpochFormat format = EpochFormat.Seconds)
        {
            var t = dt - new DateTime(1970, 1, 1);

            switch (format)
            {
                case EpochFormat.Milliseconds:
                    return (long) t.TotalMilliseconds;
                case EpochFormat.Seconds:
                default:
                    return (long)t.TotalSeconds;
            }
        }
    }
}
