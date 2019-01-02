using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AiWPF
{
    /// <summary>
    /// Interaction logic for BusyIndicator.xaml
    /// </summary>
    public partial class BusyIndicator : Window
    {
        public BusyIndicator()
        {
            InitializeComponent();

            BusyIndicator1.IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        BusyIndicator1.BusyContent = "Loading ...";
                    }));
                    System.Threading.Thread.Sleep(1);           // Osht e nevojshme spaku 1ms vonese
                }
            }).ContinueWith(task => { BusyIndicator1.IsBusy = false; }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
