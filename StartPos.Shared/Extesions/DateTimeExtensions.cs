using System;
using System.Collections.Generic;

namespace StartPos.Shared.Extesions
{
    public static class DateTimeExtensions
    {
        public static string GetShortDay(this DateTime dateTime)
        {
            return ShortDayDict[dateTime.DayOfWeek];
        }

        private static readonly Dictionary<DayOfWeek, string> ShortDayDict = new Dictionary<DayOfWeek, string>()
        {
            {DayOfWeek.Monday, "Pn"},
            {DayOfWeek.Tuesday, "Wt"},
            {DayOfWeek.Wednesday, "Sr"},
            {DayOfWeek.Thursday, "Cz"},
            {DayOfWeek.Friday, "Pt"},
            {DayOfWeek.Saturday, "So"},
            {DayOfWeek.Sunday, "Nd"}
        };
    }
}