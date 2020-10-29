using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SerialButtonLogger
{
    public class Logger : PropertyChangedAware
    {
        private static readonly string _fileName = "SerialButtonLog.csv";

        private object _fileLock = new object();
        public ObservableMonthDataCollection Months { get; } = new ObservableMonthDataCollection();

        public event EventHandler<LogMessage> LogMessageAvailable;

        private string _filePath = string.Empty;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if(SetProperty(ref _filePath, value, nameof(FilePath)))
                {
                    GetMonths();
                }
            }
        }

        public Dispatcher Dispatcher { get; set; } = null;

        private string FullPath
        {
            get
            {
                return string.Format("{0}{1}{2}", FilePath, FilePath.EndsWith("\\") ? "" : "\\", _fileName);
            }
        }


        private void GetMonths()
        {
            if(File.Exists(FullPath))
            {
                string[] lines;
                lock (_fileLock)
                {
                    lines = File.ReadAllLines(FullPath);
                }

                List<string> monthLines = new List<string>();
                foreach(string line in lines)
                {
                    monthLines.Add(line);
                    if (line == "</TimeLog>")
                    {
                        try
                        {
                            Months.Add(MonthData.FromLines(monthLines));
                        }
                        catch (Exception ex)
                        {
                            LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Error, Message = ex.Message });
                        }
                        monthLines.Clear();
                    }
                }
                int monthcount = Months.Count;
            }
        }

        internal void OnCtsSwitch(object sender, CTSSwitchEventArgs args)
        {
            if(false == args.CTSHolding)
            {
                try
                {
                    AddStamp(args.Time);
                }
                catch (Exception ex)
                {
                    LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Error, Message = ex.Message });
                }
            }
        }

        internal void AddStamp(DateTime stamp)
        {
            if (null != Dispatcher && !Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => AddStamp(stamp));
            }
            else
            {
                Months.GetOrCreate(stamp.Year, stamp.Month).AddStamp(stamp);

                lock (_fileLock)
                {
                    File.WriteAllLines(FullPath, Months.SelectMany(p => p.Value.ToLines()));
                }
            }
        }
    }
}
