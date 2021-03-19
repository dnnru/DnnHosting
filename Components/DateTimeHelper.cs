#region

using System;
using System.Globalization;
using System.Linq;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public static class DateTimeHelper
    {
        public static DateTime? ToDateTime(this object value, CultureInfo culture = null)
        {
            if (value == null)
            {
                return null;
            }

            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }

            try
            {
                DateTime dt = Convert.ToDateTime(value);
                if (dt != DateTime.MinValue)
                {
                    return dt;
                }
            }
            catch
            {
                //ignore
            }

            string strValue = value is Array array && array.Length > 0 ? array.GetValue(0).ToCultureSpecificString(culture).Trim() : value.ToCultureSpecificString(culture).Trim();

            if (string.IsNullOrWhiteSpace(strValue) || strValue.ToLower() == "null")
            {
                return null;
            }

            if (long.TryParse(strValue, out var l))
            {
                return FromUnixTime(l);
            }

            DateTime result = strValue.AsDateTime(culture, DateTime.MinValue);

            return result == DateTime.MinValue ? null : (DateTime?) result;
        }

        public static DateTime AsDateTime(this string value, CultureInfo culture, DateTime defaultValue)
        {
            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }

            string[] fmts = {"yyyy-MM-dd HH:mm:ssZ"};
            if (!culture.Name.Equals("ru-ru", StringComparison.OrdinalIgnoreCase))
            {
                fmts = fmts.Union(new CultureInfo("ru-RU").DateTimeFormat.GetAllDateTimePatterns()).ToArray();
            }

            fmts = fmts.Union(culture.DateTimeFormat.GetAllDateTimePatterns()).ToArray();
            fmts = Equals(culture, CultureInfo.InvariantCulture)
                       ? fmts.Union(CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns()).ToArray()
                       : fmts.Union(CultureInfo.InvariantCulture.DateTimeFormat.GetAllDateTimePatterns()).ToArray();
            if (!string.IsNullOrEmpty(value) && DateTime.TryParseExact(value, fmts, culture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        public static DateTime FromUnixTime(int unixTime)
        {
            if (unixTime <= 0)
            {
                return DateTime.Now;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            if (unixTime <= 0)
            {
                return DateTime.Now;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static int FromUnixTimeDaysPeriod(long unixTime)
        {
            return DateTime.Now.Date.Subtract(FromUnixTime(unixTime).Date).Duration().Days + 1;
        }

        public static int ToUnixTimeInt(DateTime date)
        {
            if (date == default)
            {
                return 0;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32((date - epoch).TotalSeconds);
        }

        public static long ToUnixTimeLong(DateTime date)
        {
            if (date == default)
            {
                return 0;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
    }
}