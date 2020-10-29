using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SerialButtonLogger
{
    public class ObservableMonthDataCollection : Dictionary<int, MonthData>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private static int MonthIndex(int year, int month)
        {
            return year * 12 + month - 1;
        }

        public void Add(MonthData monthData)
        {
            int key = MonthIndex(monthData.Year, monthData.Month);
            if(ContainsKey(key))
            {
                this[key] = monthData;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, monthData));
            }
            else
            {
                Add(key, monthData);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, monthData));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }

        public MonthData GetOrCreate(int year, int month)
        {
            if(!ContainsKey(MonthIndex(year, month)))
            {
                Add(new MonthData() { Year = year, Month = month });
            }
            return this[MonthIndex(year, month)];
        }

        public MonthData CurrentMonth
        {
            get
            {
                DateTime now = DateTime.Now;
                return GetOrCreate(now.Year, now.Month);
            }
        }
    }
}
