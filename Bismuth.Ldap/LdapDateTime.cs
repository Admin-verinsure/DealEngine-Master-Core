using System;

namespace Bismuth.Ldap
{
    public class LdapDateTime
    {
        public int Year { get; protected set; }

        public int Month { get; protected set; }

        public int Day { get; protected set; }

        public int Hour { get; protected set; }

        public int Minute { get; protected set; }

        public int Second { get; protected set; }

        public LdapDateTime (int year, int month, int day)
            : this (year, month, day, 0, 0, 0)
        { }

        public LdapDateTime (int year, int month, int day, int hour, int minute, int second)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public LdapDateTime (DateTime dateTime)
            : this (dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second)
        { }

        public string ToString(string format)
        {
            DateTime dt = new DateTime(Year, Month, Day, Hour, Minute, Second);
            return dt.ToString(format);
        }

        public static LdapDateTime Now {
            get { return new LdapDateTime(DateTime.Now); }
        }

        public static LdapDateTime UtcNow {
            get { return new LdapDateTime(DateTime.UtcNow); }
        }

        public static LdapDateTime MinValue {
            get { return new LdapDateTime(DateTime.MinValue); }
        }

        public static LdapDateTime MaxValue {
            get { return new LdapDateTime(DateTime.MaxValue); }
        }

        public static implicit operator LdapDateTime (DateTime dt)
        {
            return new LdapDateTime(dt);
        }

        public static implicit operator string (LdapDateTime ldt) 
        {
            return ldt.ToString("yyyyMMddHHmmss") + "Z";
        }
    }
}

