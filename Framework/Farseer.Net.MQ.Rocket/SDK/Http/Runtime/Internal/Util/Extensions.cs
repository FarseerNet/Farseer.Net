using System;
using System.Diagnostics;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Util
{
    internal static class Extensions
    {
        private static readonly long   ticksPerSecond = TimeSpan.FromSeconds(value: 1).Ticks;
        private static readonly double tickFrequency  = ticksPerSecond / (double)Stopwatch.Frequency;

        public static long GetElapsedDateTimeTicks(this Stopwatch self)
        {
            double stopwatchTicks = self.ElapsedTicks;
            var    ticks          = (long)(stopwatchTicks * tickFrequency);
            return ticks;
        }
    }
}