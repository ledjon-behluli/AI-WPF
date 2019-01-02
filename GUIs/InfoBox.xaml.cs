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
using AiWPF.Enums;
using System.Windows.Threading;

namespace AiWPF
{
    /// <summary>
    /// Interaction logic for InfoBox.xaml
    /// </summary>
    public partial class InfoBox : Window
    {
        private ThreadedInfoBox TinfoBox;
        private readonly Operation OperationType;         

        public InfoBox(ThreadedInfoBox _tInfoBox, Operation _OperationType, string DisplayText)
        {
            InitializeComponent();
                        
            this.Title = _tInfoBox.Title;
            TinfoBox = _tInfoBox;
            OperationType = _OperationType;       
            lblText.Content = DisplayText;

            btnClose.Click += (s, e) => this.Close();
            btnCancel.Click += (s, e) => { TinfoBox.Canceled?.Invoke((OperationType == Operation.Creating) ? Operation.Creating : Operation.Processing); this.Close(); };
            
            TinfoBox.DisplayTextChanged += (text) => this.Dispatcher.BeginInvoke(new Action(() => 
                {
                    lblText.Content = text;

                    if (text.Split(':')[0] == "Number of steps traveled")
                    {
                        try
                        {
                            btnCancel.IsEnabled = false;
                            img.Margin = new Thickness(28, 10, 20, 35);
                            WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(img, new System.Windows.Media.Animation.RepeatBehavior(1));
                            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(img, new BitmapImage(new Uri("pack://application:,,,/Resources/done.gif")));
                        }
                        catch { }
                    }
                }));                           
        }        
    }
}
