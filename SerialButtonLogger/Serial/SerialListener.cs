using System;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace SerialButtonLogger
{
    public class SerialListener : PropertyChangedAware, IDisposable
    {
        public static readonly string EmptyPort = "select Port...";
        private static readonly double _defaultDelayMilliseconds = 4;

        private SerialPort _serialPort = new SerialPort();
        private DelayedAction _loggingAction;

        public event CTSSwitchHandler CTSSwitch;
        public event EventHandler<LogMessage> LogMessageAvailable;

        public ObservableCollection<string> Ports { get; private set; } = new ObservableCollection<string>() { EmptyPort };

        public string SelectedPort
        {
            get { return _serialPort.PortName; }
            set
            {
                if(value != _serialPort.PortName && Ports.Contains(value))
                {
                    _serialPort.Close();
                    _serialPort.PortName = value;
                    if(EmptyPort != value)
                    {
                        OpenPort();
                    }
                    OnPropertyChanged(nameof(SelectedPort));
                }
            }
        }

        public SerialListener()
        {
            _loggingAction = new DelayedAction(PollCTS, _defaultDelayMilliseconds);

            _serialPort.PortName = EmptyPort;
            _serialPort.RtsEnable = true;

            _serialPort.PinChanged += OnPinChanged;
            _serialPort.ErrorReceived += OnError;

            foreach (string portName in SerialPort.GetPortNames())
            {
                Ports.Add(portName);
            }
        }

        private void OnError(object sender, SerialErrorReceivedEventArgs e)
        {
            LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Error, Message = string.Format("Serial error: {0}", e.EventType.ToString()) });
            ClosePort();
        }

        internal void PollCTS()
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    CTSSwitch?.Invoke(this, new CTSSwitchEventArgs() { CTSHolding = _serialPort.CtsHolding });
                }
                catch (UnauthorizedAccessException ex)
                {
                    LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Warning, Message = string.Format("{0}: Device disconnected: {1}", DateTime.Now, ex.Message) });
                    ClosePort();
                }
            }
            else
            {
                LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Warning, Message = string.Format("{0}: Poll: Serial Port not open.", DateTime.Now) });
            }
        }

        private void OnPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if(null == e)
            {
                return;
            }
            try
            {
                switch (e.EventType)
                {
                    case SerialPinChange.CtsChanged:
                        _loggingAction.Fire();
                        break;
                    case SerialPinChange.CDChanged:
                        if(!_serialPort.CDHolding)
                        {
                            ClosePort();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Warning, Message = string.Format("{0}: Device disconnected: {1}", DateTime.Now, ex.Message) });
                ClosePort();
            }
        }

        private void ClosePort()
        {
            _serialPort.Close();
            LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Info, Message = string.Format("{0} closed.", SelectedPort) });
            SelectedPort = EmptyPort;
        }

        private void OpenPort()
        {
            try
            {
                _serialPort.Open();
                LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Info, Message = string.Format("{0} open for listening.", SelectedPort) });
            }
            catch (Exception ex)
            {
                LogMessageAvailable?.Invoke(this, new LogMessage() { MessageSeverity = LogMessage.Severity.Info, Message = string.Format("Unable to open {0} for listening: {1}", SelectedPort, ex.Message) });
            }
        }

        public void Dispose()
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            _serialPort.Dispose();
        }
    }
}
