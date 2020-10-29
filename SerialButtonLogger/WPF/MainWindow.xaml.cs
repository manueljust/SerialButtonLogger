using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialButtonLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: detect dockingstation disconnect
        // https://stackoverflow.com/questions/12686820/programmatic-way-to-see-if-a-mobile-computer-laptop-tablet-etc-is-connected-t
        // and/or detect serial disconnect

        public string ApplicationName { get; } = "Serial Button Logger";
        public SerialListener Listener { get; } = new SerialListener();
        public Logger Logger { get; } = new Logger();

        private TrayIcon _notifyIcon = null;
        private bool _doExit = false;

        public MainWindow()
        {
            this.HideMinimizeButton();

            Logger.Months.CollectionChanged += UpdateMonths;

            InitializeComponent();

            Logger.Dispatcher = Dispatcher;
            Logger.LogMessageAvailable += _notifyIcon.OnLogMessage;
            Listener.LogMessageAvailable += _notifyIcon.OnLogMessage;
            Listener.CTSSwitch += Logger.OnCtsSwitch;
        }

        private void UpdateMonths(object sender, EventArgs e)
        {
            MonthPanel.Children.Clear();
            foreach (MonthData data in Logger.Months.OrderBy(m => -m.Key).Select(m => m.Value))
            {
                MonthPanel.Children.Add(new MonthDataView(data));
            }
        }

        private void thisMainWindow_Initialized(object sender, EventArgs e)
        {
            _notifyIcon = new TrayIcon(this, ApplicationName);

            Listener.SelectedPort = (string)Properties.Settings.Default["DefaultPort"];
            Logger.FilePath = (string)Properties.Settings.Default["FilePath"];
        }

        private void thisMainWindow_Closing(object sender, CancelEventArgs e)
        {

            if (_doExit)
            {
                // do close when exit requested
            }
            else
            { 
                // otherwise just hide and notify
                e.Cancel = true;
                thisMainWindow.Hide();
                _notifyIcon.OnLogMessage(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Info, Message = "Running in Background." });
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        internal void Exit()
        {
            _doExit = true;
            Properties.Settings.Default["DefaultPort"] = Listener.SelectedPort;
            Properties.Settings.Default["FilePath"] = Logger.FilePath;
            Properties.Settings.Default.Save();
            _notifyIcon?.Dispose();
            Listener.Dispose();
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debugger.Break();
        }

        private void AddCustomStamp_Click(object sender, RoutedEventArgs e)
        {
            DateTime t = DateTime.Now;

            AddTimeStampWindow w = new AddTimeStampWindow();
            w.Closing += (o, args) => { t = w.Date; };

            if(true == w.ShowDialog())
            {
                Logger.AddStamp(t);
            }
        }
    }
}
