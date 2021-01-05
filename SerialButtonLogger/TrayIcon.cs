using Hardcodet.Wpf.TaskbarNotification;
using SerialButtonLogger.Serial;
using SerialButtonLogger.Util;
using SerialButtonLogger.WPF;
using System.Windows;
using System.Windows.Controls;

namespace SerialButtonLogger
{
    public class TrayIcon : TaskbarIcon
    {
        private MainWindow _mainWindow = null;
        private string _balloonTitle = string.Empty;

        public TrayIcon(MainWindow mainWindow, string applicationName)
        {
            _mainWindow = mainWindow;
            _balloonTitle = applicationName;

            ToolTipText = applicationName;
            Visibility = Visibility.Visible;
            Icon = Properties.Resources.Stopwatch;
            MenuActivation = PopupActivationMode.RightClick;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(new MenuItem() { Header = "Show Window", Command = new ActionCommand(_mainWindow.Show) });
            ContextMenu.Items.Add(new MenuItem() { Header = "Trigger Switch", Command = new ActionCommand(() => { _mainWindow.Logger.OnCtsSwitch(this, new CTSSwitchEventArgs()); }) });
            ContextMenu.Items.Add(new MenuItem() { Header = "Exit", Command = new ActionCommand(_mainWindow.Exit) });

            LeftClickCommand = new ActionCommand(_mainWindow.Show);
            DoubleClickCommand = new ActionCommand(_mainWindow.Show);
        }

        internal void OnLogMessage(object sender, LogMessage e)
        {
            switch(e.MessageSeverity)
            {
                case LogMessage.Severity.Info:
                    ShowBalloonTip(_balloonTitle, e.Message, BalloonIcon.Info);
                    break;
                case LogMessage.Severity.Warning:
                    ShowBalloonTip(_balloonTitle, e.Message, BalloonIcon.Warning);
                    break;
                case LogMessage.Severity.Error:
                    ShowBalloonTip(_balloonTitle, e.Message, BalloonIcon.Error);
                    break;
                case LogMessage.Severity.Log:
                default:
                    // do nothing
                    break;
            }
        }
    }
}
