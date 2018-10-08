using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace DbModel.Extensions
{
    public class Cultures
    {
        public static void InitializePersianCulture()
        {
            InitializeCulture("fa-ir", new[] { "ی", "د", "س", "چ", "پ", "ج", "ش" },
                              new[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" },
                              new[]
                                  {
                                      "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی",
                                      "بهمن", "اسفند", ""
                                  },
                              new[]
                                  {
                                      "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی",
                                      "بهمن", "اسفند", ""
                                  }, "ق.ظ. ", "ب.ظ. ", "yyyy/MM/dd", new PersianCalendar());
        }

        public static void InitializeCulture(string culture, string[] abbreviatedDayNames, string[] dayNames,
                                             string[] abbreviatedMonthNames, string[] monthNames, string amDesignator,
                                             string pmDesignator, string shortDatePattern, Calendar calendar)
        {
            var calture = new CultureInfo(culture);
            var info = calture.DateTimeFormat;
            info.AbbreviatedDayNames = abbreviatedDayNames;
            info.DayNames = dayNames;
            info.AbbreviatedMonthNames = abbreviatedMonthNames;
            info.MonthNames = monthNames;
            info.AMDesignator = amDesignator;
            info.PMDesignator = pmDesignator;
            info.ShortDatePattern = shortDatePattern;
            info.FirstDayOfWeek = DayOfWeek.Saturday;
            var cal = calendar;
            var type = typeof(DateTimeFormatInfo);
            var fieldInfo = type.GetField("calendar", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
                fieldInfo.SetValue(info, cal);
            var field = typeof(CultureInfo).GetField("calendar", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(calture, cal);
            Thread.CurrentThread.CurrentCulture = calture;
            Thread.CurrentThread.CurrentUICulture = calture;
            CultureInfo.CurrentCulture.DateTimeFormat = info;
            CultureInfo.CurrentUICulture.DateTimeFormat = info;
        }
    }
}
