using System;
using System.Globalization;

namespace techcertain2015rebuildcore.Helpers
{
	public static class DateTimeHelper
	{
		// from https://weblog.west-wind.com/posts/2015/Feb/10/Back-to-Basics-UTC-and-TimeZones-in-NET-Web-Apps

		/// <summary>
		/// Returns TimeZone adjusted time for a given from a Utc or local time.
		/// Date is first converted to UTC then adjusted.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="timeZoneId"></param>
		/// <returns></returns>
		public static DateTime ToTimeZoneTime (this DateTime time, string timeZoneId = "Pacific Standard Time")
        {
			TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById (timeZoneId);
			return time.ToTimeZoneTime (tzi);
		}

		/// <summary>
		/// Returns TimeZone adjusted time for a given from a Utc or local time.
		/// Date is first converted to UTC then adjusted.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="timeZoneId"></param>
		/// <returns></returns>
		public static DateTime ToTimeZoneTime (this DateTime time, TimeZoneInfo tzi)
		{
			return TimeZoneInfo.ConvertTimeFromUtc (time, tzi);
		}

		/// <summary>
		/// Converts the current DateTime to UTC based on the specified Time Zone.
		/// </summary>
		/// <returns>The universal time.</returns>
		/// <param name="time">Time.</param>
		/// <param name="tzi">Tzi.</param>
		public static DateTime ToUniversalTime (this DateTime time, TimeZoneInfo tzi)
		{
			return TimeZoneInfo.ConvertTimeToUtc (time, tzi);
		}
	}
}

