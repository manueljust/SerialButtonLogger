using SerialButtonLogger.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SerialButtonLogger
{
    public class MonthData : PropertyChangedAware
    {
        private static readonly char _csvDelimiter = ',';

        private int _year = 1970;
        public int Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value, nameof(Year)); }
        }

        private int _month = 1;
        public int Month
        {
            get { return _month; }
            set { SetProperty(ref _month, value, nameof(Month)); }
        }

        public ObservableCollection<DayData> DayData { get; } = new ObservableCollection<DayData>(new DayData[31].PopulateDefaultConstructor());

        public void AddStamp(DateTime stamp)
        {
            if (stamp.Year == Year && stamp.Month == Month)
            {
                DayData[stamp.Day - 1].AddStamp(stamp);
            }
        }

        public double TotalHours(int dayIndex)
        {
            return DayData[dayIndex].TotalHours;
        }

        public static MonthData FromLines(List<string> lines)
        {
            // expected:
            // opener
            // date line
            // hours line
            // warning line
            // x timestamps lines
            // closer
            if (5 > lines.Count)
            {
                throw new ArgumentException("Error parsing Month data: At least 5 lines are expected for Month data.");
            }

            MonthData data = new MonthData();
            data.Year = int.Parse(lines[0].Substring(15, 4));
            data.Month = int.Parse(lines[0].Substring(28, 2));

            for (int i = 4; i < lines.Count - 1; i++)
            {
                string[] fields = lines[i].Split(_csvDelimiter);
                if (DateTime.DaysInMonth(data.Year, data.Month) != fields.Length)
                {
                    throw new ArgumentException(string.Format("Error parsing Month data: {0}.{1} should have {2} days. Only {3} fields in line {4} found.", data.Month, data.Year, DateTime.DaysInMonth(data.Year, data.Month), fields.Length, i + 1));
                }
                for (int day = 0; day < fields.Length; day++)
                {
                    if (string.Empty != fields[day])
                    {
                        string field = fields[day];
                        data.AddStamp(new DateTime(
                            data.Year, data.Month, day + 1,
                            int.Parse(fields[day].Substring(0, 2)),
                            int.Parse(fields[day].Substring(3, 2)),
                            int.Parse(fields[day].Substring(6, 2))));
                    }
                }
            }

            // verify data
            string[] hours = lines[2].Split(_csvDelimiter);
            for (int day = 0; day < hours.Length; day++)
            {
                if (0.01 < Math.Abs(double.Parse(hours[day]) - data.TotalHours(day)))
                {
                    throw new ArgumentException(string.Format("Error parsing Month data: {0}.{1}.{2} should have {3} hours from stamps, but has {4} hours from totalhours.", data.Month, data.Year, day + 1, data.TotalHours(day), double.Parse(hours[day])));
                }
            }

            return data;
        }

        public IEnumerable<string> ToLines()
        {
            List<string> lines = new List<string>();
            List<string> fields = new List<string>();

            // title line
            lines.Add(string.Format("<TimeLog Year=\"{0:D4}\" Month=\"{1:D2}\">", Year, Month));
            // date line
            for (DateTime d = new DateTime(Year, Month, 1); d.Month == Month; d += TimeSpan.FromDays(1))
            {
                fields.Add(d.ToString("dd.MM"));
            }
            lines.Add(string.Join(_csvDelimiter.ToString(), fields));
            fields.Clear();
            // total hours  
            for (int i = 0; i < DateTime.DaysInMonth(Year, Month); i++)
            {
                fields.Add(DayData[i].TotalHours.ToString("0.#####"));
            }
            lines.Add(string.Join(_csvDelimiter.ToString(), fields));
            fields.Clear();
            // warning line
            for (int i = 0; i < DateTime.DaysInMonth(Year, Month); i++)
            {
                fields.Add(0 != DayData[i].Stamps.Count % 2 ? "Odd Stamp Number!" : "");
            }
            lines.Add(string.Join(_csvDelimiter.ToString(), fields));
            fields.Clear();
            // stamps lines
            for (int i = 0; i < DayData.Max(d => d.Stamps.Count); i++)
            {
                for (int j = 0; j < DateTime.DaysInMonth(Year, Month); j++)
                {
                    fields.Add(i < DayData[j].Stamps.Count ? DayData[j].Stamps[i].ToString("HH:mm:ss") : "");
                }
                lines.Add(string.Join(_csvDelimiter.ToString(), fields));
                fields.Clear();
            }
            lines.Add("</TimeLog>");
            return lines;
        }
    }
}
