using System;
using System.Diagnostics;

namespace FreakinStocksUI.Models
{
    class TimerPro : IDisposable
    {
        public Stopwatch Timer { get; set; } = new();

        public TimerPro()
        {
            Timer.Start();
        }

        public void Dispose()
        {
            Timer.Stop();
            Debug.WriteLine(Timer.Elapsed);
        }
    }
}
