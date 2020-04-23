/* Name: L. Yao
 * Date: November 4, 2019
 * Desc: A library containing date, weather, and season functions and classes for the simulation */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DateWeatherSeasonLib
{
	[System.Serializable]
	public class Date
	{
		public int year, month, day;
	}

	public static class DateFuncs
	{
		public static int[] daysInMonths = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

		public static Date NextDay(Date date)
		{
			date.day++;
			if (date.day > daysInMonths[date.month - 1])
			{
				date.day = 1;
				date.month++;
			}
			if (date.month > 12)
			{
				date.month = 1;
				date.year++;
			}
			return date;
		}

		public static Date GetFutureDate(Date curDate, int numDays, int numWeeks = 0, int numMonths = 0, int numYears = 0)
		{
			// For some reason setting futureDate to be equal to curDate just makes everything done to it also apply to curDate, even though it's not being passed by reference.
			// (Maybe the compiler is just "optimizing" it by making it pass by reference?)
			Date futureDate = new Date
			{
				year = curDate.year,
				month = curDate.month,
				day = curDate.day
			};

			futureDate.year += numYears;

			futureDate.month += numMonths;
			while (futureDate.month > 12)
			{
				futureDate.month -= 12;
				futureDate.year++;
			}

			numDays += numWeeks * 7;
			for (int i = 0; i < numDays; i++)
			{
				futureDate = NextDay(futureDate);
			}

			return futureDate;
		}

		// An equals operator needs to be defined for the Date class, but I can't put it in the class itself or else it won't be serializable.
		public static bool Equals(Date d1, Date d2)
		{
			if (d1.day == d2.day)
			{
				if (d1.month == d2.month)
				{
					if (d1.year == d2.year)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Unsurprisingly, returns the number of days until a future date.
		public static int DaysUntil(Date curDate, Date futureDate)
		{
			int days = 0;
			Date tempDate = new Date // Done this way for the same reason as above
			{
				year = curDate.year,
				month = curDate.month,
				day = curDate.day
			};
			//Debug.Log(DateToString(curDate) + " " + DateToString(futureDate));
			while (!Equals(tempDate, futureDate))
			{
				tempDate = NextDay(tempDate);
				days++;
				//Debug.Log(DateToString(tempDate));
				//Debug.Log(days);
			}
			return days;
		}

		public static string DateToString(Date date)
		{
			return string.Format("Year {0}, Month {1}, Day {2}", date.year, date.month, date.day);
		}
	}

	[System.Serializable]
	public enum Weather { Clear, Cloudy, Foggy, Rainy, Stormy }

	[System.Serializable]
	public enum Season { Spring, Summer, Autumn, Winter }

	public static class ClimateFuncs
	{
		public static Weather GetRandomWeather()
		{
			Weather[] weatherTable = { Weather.Clear, Weather.Cloudy, Weather.Foggy, Weather.Rainy, Weather.Stormy };
			return weatherTable[Random.Range(0, weatherTable.Length)];
		}

		// Seasons are assumed to last three months, and change on the 21st of a month.
		public static Season GetSeason(Date date)
		{
			if ((date.month == 12 && date.day >= 21) || (date.month == 3 && date.day < 21) || (date.month < 3))
			{
				return Season.Winter;
			}
			else if ((date.month == 3 && date.day >= 21) || (date.month == 6 && date.day < 21) || (date.month < 6))
			{
				return Season.Spring;
			}
			else if ((date.month == 6 && date.day >= 21) || (date.month == 9 && date.day < 21) || (date.month < 9))
			{
				return Season.Summer;
			}
			else
			{
				return Season.Autumn;
			}
		}
	}
}