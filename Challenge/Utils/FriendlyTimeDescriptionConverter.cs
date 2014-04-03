using System;
using System.Diagnostics;
using System.Windows.Data;
using System.Globalization;
using ChallengeApp.Resources;

namespace ChallengeApp.Utils
{
    public class FriendlyTimeDescriptionConverter : IValueConverter {
        const int SECOND    = 1;
        const int MINUTE    = 60 * SECOND;
        const int HOUR      = 60 * MINUTE;
        const int DAY       = 24 * HOUR;
        const int MONTH     = 30 * DAY;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";

            long post_timestamp = (long)(Double.Parse(value.ToString()) / 1000);
            long now_timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            int delta           = (int)(now_timestamp - post_timestamp);
            TimeSpan ts_delta   = new TimeSpan(0, 0, delta);

            if (delta < 0) return "";
            if (delta < 30  * SECOND)   return AppResources.JustNow;
            if (delta < 2   * MINUTE)   return AppResources.AMinuteAgo;
            if (delta < 45  * MINUTE)   return String.Format(AppResources.NMinutesAgo, ts_delta.Minutes);
            if (delta < 120 * MINUTE)   return AppResources.AnHourAgo;
            if (delta < 24  * HOUR)     return String.Format(AppResources.NHoursAgo, ts_delta.Hours);
            if (delta < 48  * HOUR)     return AppResources.Yesterday;
            if (delta < 7   * DAY)      return String.Format(AppResources.NDaysAgo, ts_delta.Days);
            if (delta < 14  * DAY)      return AppResources.OneWeekAgo;
            if (delta < 30  * DAY)      return String.Format(AppResources.NWeeksAgo, Math.Round((double)(ts_delta.Days / 7)));
            if (delta < 12  * MONTH)
            {
                int months = System.Convert.ToInt32(Math.Floor((double)ts_delta.Days / 30));
                return months <= 1 ? AppResources.OneMonthAgo : String.Format(AppResources.NMonthsAgo, months);
            }
            else
            {
                int years = System.Convert.ToInt32(Math.Floor((double)ts_delta.Days / 365));
                return years <= 1 ? AppResources.OneYearAgo : String.Format(AppResources.NYearsAgo, years);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? "" : value.ToString();
        }

        protected DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}