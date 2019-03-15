using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class DateTimeEx
  {
    private static readonly double MsSqlTimeResolutionMs = 4.0;
    public static readonly string PreciseFormat = "dd-MM-yyyy H:mm:ss.fff";
    public static readonly DateTime MinDateTime = new DateTime(1900, 1, 1, 12, 0, 0);

    public static bool IsToday(DateTime dateTime)
    {
      return SManagedDateTime.Today == dateTime.Date;
    }

    public static bool IsYesterday(DateTime dateTime)
    {
      return (SManagedDateTime.Today - dateTime.Date).Days == 1;
    }

    public static bool IsTheSameDay(DateTime dateTime1, DateTime dateTime2)
    {
      if (dateTime1.Year == dateTime2.Year && dateTime1.Month == dateTime2.Month)
        return dateTime1.Day == dateTime2.Day;
      return false;
    }

    public static bool IsTheSameTime(DateTime dateTime1, DateTime dateTime2)
    {
      return Math.Abs((dateTime1 - dateTime2).TotalMilliseconds) < DateTimeEx.MsSqlTimeResolutionMs;
    }

    public static bool IsTheSameTimeExcludeMs(DateTime dateTime1, DateTime dateTime2)
    {
      if (dateTime1.Date == dateTime2.Date && dateTime1.Hour == dateTime2.Hour && dateTime1.Minute == dateTime2.Minute)
        return dateTime1.Second == dateTime2.Second;
      return false;
    }

    public static bool IsTheSameOrGreaterTime(DateTime dateTime1, DateTime dateTime2)
    {
      if (dateTime1 > dateTime2)
        return true;
      return DateTimeEx.IsTheSameTime(dateTime1, dateTime2);
    }

    public static bool IsTheSameTimeExcludeMilliseconds(DateTime dateTime1, DateTime dateTime2)
    {
      return Math.Abs((dateTime1 - dateTime2).TotalSeconds) < 1.0;
    }

    public static bool IsDayTime(DateTime dateTime, TimeSpan time)
    {
      if (dateTime.Hour == time.Hours)
        return dateTime.Minute == time.Minutes;
      return false;
    }

    public static string ToInvariantCultureString(this DateTime value)
    {
      return value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static DateTime Min(DateTime dateTime1, DateTime dateTime2)
    {
      return new DateTime(Math.Min(dateTime1.Ticks, dateTime2.Ticks));
    }

    public static DateTime Max(DateTime dateTime1, DateTime dateTime2)
    {
      return new DateTime(Math.Max(dateTime1.Ticks, dateTime2.Ticks));
    }

    public static DateTime RoundUp(this DateTime dt, TimeSpan d)
    {
      long num = (d.Ticks - dt.Ticks % d.Ticks) % d.Ticks;
      return new DateTime(dt.Ticks + num, dt.Kind);
    }

    public static DateTime RoundDown(this DateTime dt, TimeSpan d)
    {
      long num = dt.Ticks % d.Ticks;
      return new DateTime(dt.Ticks - num, dt.Kind);
    }

    public static DateTime SqlClamp(this DateTime dt)
    {
      if (dt > SqlDateTime.MaxValue.Value)
        return SqlDateTime.MaxValue.Value;
      if (dt < SqlDateTime.MinValue.Value)
        return SqlDateTime.MinValue.Value;
      return dt;
    }

    public static DateTime TranslateRemotingFakeLocalTimeToUtc(
      this DateTime dateTime,
      TimeSpan localUtcOffset,
      TimeSpan remoteUtcOffset)
    {
      TimeSpan timeSpan = remoteUtcOffset - localUtcOffset;
      return new DateTime(dateTime.Add(timeSpan).Ticks, DateTimeKind.Utc);
    }

    public static TimeSpan Duration(this DateTime self, DateTime other)
    {
      return (self - other).Duration();
    }
  }
}
