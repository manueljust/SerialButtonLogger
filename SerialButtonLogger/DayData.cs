using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SerialButtonLogger
{
    public class DayData : PropertyChangedAware
    {
        public ObservableCollection<DateTime> Stamps { get; set; } = new ObservableCollection<DateTime>();

        public double TotalHours
        {
            get
            {
                double hours = 0;
                DateTime checkin = DateTime.MinValue;
                foreach (DateTime stamp in new SortedSet<DateTime>(Stamps))
                {
                    if (DateTime.MinValue == checkin)
                    {
                        checkin = stamp;
                    }
                    else
                    {
                        hours += (stamp - checkin).TotalHours;
                        checkin = DateTime.MinValue;
                    }
                }
                return hours;
            }
        }

        public void AddStamp(DateTime stamp)
        {
            Stamps.Add(new DateTime(stamp.Year, stamp.Month, stamp.Day, stamp.Hour, stamp.Minute, stamp.Second));

            // TotalHours will only change for even number of stamps
            if (0 == Stamps.Count % 2)
            {
                OnPropertyChanged(nameof(TotalHours));
            }
        }
    }
}
