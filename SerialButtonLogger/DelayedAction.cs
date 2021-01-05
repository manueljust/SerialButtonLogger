using System;
using System.Timers;

namespace SerialButtonLogger
{
    public class DelayedAction
    {
        private Timer _timer = null;
        private int _fireCounter = 0;

        public DelayedAction(Action callback, TimeSpan delay)
        {
            _timer = new Timer(delay.TotalMilliseconds);
            _timer.Elapsed += (o, e) =>
            {
                _timer.Stop();
                callback.Invoke();
                System.Diagnostics.Debug.WriteLine("DelayedAction called {0} times.", _fireCounter);
                _fireCounter = 0;
            };
        }

        public void Fire()
        {
            _fireCounter++;
            if(_timer.Enabled)
            {
                _timer.Stop();
                _timer.Start();
            }
            else
            {
                _timer.Start();
            }
        }
    }
}
