using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using AiWPF.Enums;

namespace AiWPF
{
    /// <summary>
    /// http://reedcopsey.com/2011/11/28/launching-a-wpf-window-in-a-separate-thread-part-1/
    /// </summary>
    public class ThreadedInfoBox
    {       
        public string Title; 
        private InfoBox infoBox;
        private readonly WindowStartupLocation WindowStartupLocation;
        private readonly int Left;
        private readonly int Top;
        public Action<string> DisplayTextChanged;
        public Action<Operation> Canceled;
        private static object _lock = new object();

        public ThreadedInfoBox() : this(WindowStartupLocation.CenterScreen, 0, 0) { }
        public ThreadedInfoBox(WindowStartupLocation _windowStartupLocation, int _left, int _top)
        {
            this.WindowStartupLocation = _windowStartupLocation;
            this.Left = _left;
            this.Top = _top;           
        }

        public void StartNewThreadInfoBox(Operation OperationType, string DisplayText, string Title)
        {
            Thread newInfoBoxThread = new Thread(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        this.Title = Title;
                        infoBox = new InfoBox(this, OperationType, DisplayText) {
                            WindowStartupLocation = WindowStartupLocation,
                            Left = Left,
                            Top = Top
                        };
                        infoBox.Closed += (s, args) => Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                        infoBox.Show();

                        System.Windows.Threading.Dispatcher.Run();
                    }
                    catch { }
                }
            });
            newInfoBoxThread.SetApartmentState(ApartmentState.STA);
            newInfoBoxThread.IsBackground = true;
            newInfoBoxThread.Start();        
        }

        public void EndNewThreadInfoBox()
        {
            if (infoBox != null)
                try { infoBox.Dispatcher.BeginInvoke((Action)infoBox.Close); }
                catch { }
        }        
    }
}
