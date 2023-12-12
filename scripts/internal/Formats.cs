using Godot;
using System;

#nullable enable
namespace OpenVoice
{
    public static class Formats
    {
        public static class Date
        {
            public static string GetDateFromTimeStamp(long TimeStamp, string Format = "dd.mm.yyyy")
            {
                string FormattedDate = Format;

                // Days
                if (DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Day < 10)
                { FormattedDate = FormattedDate.Replace("dd", "0" + DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Day.ToString()); }
                else FormattedDate = FormattedDate.Replace("dd", DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Day.ToString());

                // Months
                if (DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Month < 10)
                { FormattedDate = FormattedDate.Replace("mm", "0" + DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Month.ToString()); }
                else FormattedDate = FormattedDate.Replace("mm", DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Month.ToString());

                // Years
                FormattedDate = FormattedDate.Replace("yyyy", DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Year.ToString());

                return FormattedDate;
            }
        }

        public static class Time
        {
            public static string GetTimeFromTimeStamp(long TimeStamp, string Format = "hh:mm")
            {
                string FormattedDate = Format;
                
                // Hours
                if (DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Hour < 10)
                { FormattedDate = FormattedDate.Replace("hh", "0" + DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Hour.ToString()); }
                else FormattedDate = FormattedDate.Replace("hh", DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Hour.ToString());

                // Minutes
                if (DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Minute < 10)
                { FormattedDate = FormattedDate.Replace("mm", "0" + DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Minute.ToString()); }
                else FormattedDate = FormattedDate.Replace("mm", DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Minute.ToString());

                // Seconds
                if (DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Second < 10)
                { FormattedDate = FormattedDate.Replace("ss", "0" + DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Second.ToString()); }
                else FormattedDate = FormattedDate.Replace("ss", DateTimeOffset.FromUnixTimeSeconds(TimeStamp).ToUniversalTime().Second.ToString());

                return FormattedDate;
            }
        }
    }
}