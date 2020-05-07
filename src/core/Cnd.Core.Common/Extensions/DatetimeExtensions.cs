using System;
using System.Diagnostics;

namespace Cnd.Core.Common
{
    [DebuggerStepThrough]
    public static class DatetimeExtensions
    {
        public static string ToRelativeTime(this DateTime dt)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (ts.TotalSeconds > 0)
            {
                if (delta < 24 * HOUR)
                    return " today";

                if (delta < 48 * HOUR)
                    return "yesterday";

                if (delta < 30 * DAY)
                    return Math.Abs(ts.Days) + " days ago";

                if (delta < 12 * MONTH)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    months = Math.Abs(months);
                    return months <= 1 ? "one month ago" : months + " months ago";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    years = Math.Abs(years);
                    return years <= 1 ? "one year ago" : years + " years ago";
                }
            }
            else
            {
                if (delta < 24 * HOUR)
                    return " today";

                if (delta < 48 * HOUR)
                    return "tomorrow";

                if (delta < 30 * DAY)
                    return Math.Abs(ts.Days) + " days later";

                if (delta < 12 * MONTH)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    months = Math.Abs(months);
                    return months <= 1 ? "one month later" : months + " months later";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    years = Math.Abs(years);
                    return years <= 1 ? "one year later" : years + " years later";
                }
            }
        }
    }
}
