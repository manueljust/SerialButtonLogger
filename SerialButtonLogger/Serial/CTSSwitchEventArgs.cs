using System;

namespace SerialButtonLogger.Serial
{
    public delegate void CTSSwitchHandler(object sender, CTSSwitchEventArgs args);

    public class CTSSwitchEventArgs : EventArgs
    {
        public bool CTSHolding { get; set; } = false;
        public DateTime Time { get; set; } = DateTime.Now;
    }
}