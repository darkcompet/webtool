namespace Tool.Compet.Core {
	/// Extension for string.
	public static class DkDateTimes {
		public const string FMT_DATE = "yyyy-MM-dd";
		public const string FMT_TIME = "HH:mm:ss";
		public const string FMT_DATETIME = "yyyy-MM-dd HH:mm:ss";

		/// @param format: For eg,. yyyy-MM-dd HH:mm:ss
		/// Ref: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
		/// Ref: https://www.c-sharpcorner.com/blogs/date-and-time-format-in-c-sharp-programming1
		public static string FormatDk(this DateTime me, string? format = FMT_DATETIME) {
			return me.ToString(format);
		}

		/// Ref: https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=net-7.0
		/// @return Epoch ms of now (number of milliseconds has elapsed from 1970-01-01T00:00:00.000Z).
		public static long currentUnixTimeInMillis => DateTimeOffset.Now.ToUnixTimeMilliseconds();

		/// Ref: https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=net-7.0
		/// @return Epoch seconds of now (number of seconds has elapsed from 1970-01-01T00:00:00.000Z).
		public static long currentUnixTimeInSeconds => DateTimeOffset.Now.ToUnixTimeSeconds();

		/// Ref: https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=net-7.0
		/// @return Epoch ms of UTC-now (number of milliseconds has elapsed from 1970-01-01T00:00:00.000Z).
		public static long currentUnixUtcTimeInMillis => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		/// Ref: https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimeseconds?view=net-7.0
		/// @return Epoch seconds of UTC-now (number of seconds has elapsed from 1970-01-01T00:00:00.000Z).
		public static long currentUnixUtcTimeInSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		/// Convert unix time (seconds) that elapsed from epoch time.
		public static DateTime ConvertUnixTimeSecondsToUtcDatetime(long seconds) => DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;

		/// Convert unix time (milliseconds) that elapsed from epoch time.
		public static DateTime ConvertUnixTimeMillisecondsToUtcDatetime(long millis) => DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime;
	}
}
