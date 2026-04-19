using System;

public static partial class StaticUtils
{
	#region convert unix epoch <-> System.DateTime

	private static readonly DateTime epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	private static readonly DateTime maxDate = new(9999, 12, 31, 11, 59, 59, DateTimeKind.Utc);

	public static long DateTimeToUnixEpoch(DateTime dateTime)
	{
		return (long)(dateTime - epoch).TotalSeconds;
	}

	public static DateTime UnixEpochToDateTime(long unixEpoch)
	{
		var maxDt = DateTimeToUnixEpoch(maxDate);
		unixEpoch = Math.Min(unixEpoch, maxDt);
		
		return epoch.AddSeconds(unixEpoch);
	}

	#endregion

	#region next time

	// dateTime = 6:00AM 1/1/2023
	// hourInDay = 8:00AM =============> 8:00AM 1/1/2023
	// hourInDay = 5:00AM =============> 5:00AM 2/1/2013
	public static DateTime NextTime(DateTime now, string hourInDay)
	{
		var l = hourInDay.Split(':');
		var hour = StringToInt(l[0]);
		var minute = StringToInt(l[1]);

		var d = BeginDay(now).AddHours(hour).AddMinutes(minute);
		if (now < d)
		{
			return d;
		}
		else
		{
			return d.AddDays(1);
		}
	}
	
	public static DateTime NextDay(DateTime now)
	{
		return BeginDay(now).AddDays(1);
	}

	public static DateTime NextWeek(DateTime now)
	{
		return BeginWeek(now).AddDays(7);
	}

	public static DateTime NextMonth(DateTime now)
	{
		return BeginMonth(now).AddMonths(1);
	}

	public static DateTime NextYear(DateTime now)
    {
        return BeginYear(now).AddYears(1);
    }

	#endregion

	#region begin time

	public static DateTime BeginDay(DateTime now)
	{
		return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, now.Kind);
	}

	public static DateTime BeginWeek(DateTime now)
	{
		var daysBackToBeginWeek = now.DayOfWeek switch
		{
			DayOfWeek.Monday => 0,
			DayOfWeek.Tuesday => 1,
			DayOfWeek.Wednesday => 2,
			DayOfWeek.Thursday => 3,
			DayOfWeek.Friday => 4,
			DayOfWeek.Saturday => 5,
			DayOfWeek.Sunday => 6,
			_ => -1,
		};
		return BeginDay(now).AddDays(-daysBackToBeginWeek);
	}

	public static DateTime BeginMonth(DateTime now)
	{
		return new DateTime(now.Year, now.Month, 1, 0, 0, 0, now.Kind);
	}
	
	public static DateTime BeginYear(DateTime now)
	{
		return new DateTime(now.Year, 1, 1, 0, 0, 0, now.Kind);
	}

	#endregion
}