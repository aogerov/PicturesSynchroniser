using System;

namespace PicturesSynchroniser.Models
{
    public class FileNameTemplateStrings
    {
        private static readonly string[] day = { "Yes", "No" };
        private static readonly string[] date = { "YYYY.MM.DD", "MM.DD", "No date" };
        private static readonly string[] time = { "HH.MM.SS.MS", "HH.MM.SS", "HH.MM", "No time" };
        private static readonly string[] timeWithoutMiliseconds = { "HH.MM.SS", "HH.MM", "No time" };
        private static readonly string[] enumeration = { "Yes", "No" };

        public string[] Day
        {
            get
            {
                return day;
            }
        }

        public string[] Date
        {
            get
            {
                return date;
            }
        }

        public string[] TimeWithoutMiliseconds
        {
            get
            {
                return timeWithoutMiliseconds;
            }
        }

        public string[] Enumeration
        {
            get
            {
                return enumeration;
            }
        }

        public static string DayYes
        {
            get
            {
                return day[0];
            }
        }

        public static string DayNo
        {
            get
            {
                return day[1];
            }
        }

        public static string DateYearMonthDay
        {
            get
            {
                return date[0];
            }
        }

        public static string DateMonthDay
        {
            get
            {
                return date[1];
            }
        }

        public static string DateNotIncluded
        {
            get
            {
                return date[2];
            }
        }

        public static string TimeHourMinuteSecondMillisecond
        {
            get
            {
                return time[0];
            }
        }

        public static string TimeHourMinuteSecond
        {
            get
            {
                return time[1];
            }
        }

        public static string TimeHourMinute
        {
            get
            {
                return time[2];
            }
        }

        public static string TimeNotIncluded
        {
            get
            {
                return time[3];
            }
        }

        public static string EnumerationYes
        {
            get
            {
                return enumeration[0];
            }
        }

        public static string EnumerationNo
        {
            get
            {
                return enumeration[1];
            }
        }
    }
}
