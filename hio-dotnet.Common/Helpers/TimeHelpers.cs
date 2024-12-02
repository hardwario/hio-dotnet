﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Helpers
{
    public static class TimeHelpers
    {
        /// <summary>
        /// Convert Unix Time stamp in miliseconds to DateTime
        /// </summary>
        /// <param name="unixTime">timestamp in miliseconds</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(long unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)((unixTime / 1000) * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        /// <summary>
        /// Convert Unix Time stamp in miliseconds to DateTime
        /// </summary>
        /// <param name="unixTime">timestamp in miliseconds</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(UInt64 unixTime)
        {
            return UnixTimestampToDateTime((long)unixTime);
        }

        /// <summary>
        /// Convert DateTime to Unix timestamp in miliseconds
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (long)unixTimeStampInTicks / TimeSpan.TicksPerSecond * 1000;
        }
    }
}