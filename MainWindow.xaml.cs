using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private bool appIsRunning = true;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                DisplayDateAndTime();
            }).Start();

            CurrentPage.Content = new HomePageControl();

            this.Show();
            this.Closed += (s, e) => appIsRunning = false;
        }

        private bool timeStandardIsUTC = true;
        private string timeLabel = "UTC";

        private void Change_Time_Standard(object sender, MouseButtonEventArgs e)
        {
            timeStandardIsUTC = !timeStandardIsUTC;
            Swap_Time_Standard_Label();
        }

        private void Swap_Time_Standard_Label()
        {
            if (timeStandardIsUTC)
            {
                timeStandard.Text = "UTC";
                timeLabel = "UTC";
            }
            else
            {
                timeStandard.Text = "Local (PST)";
                timeLabel = "PST";
            }
            updateDateAndTime(timeStandardIsUTC ? DateTime.UtcNow : DateTime.Now);
        }

        void DisplayDateAndTime()   
        {
            while (appIsRunning)
            {
                this.Dispatcher.InvokeAsync(() =>
                { 
                    updateDateAndTime(timeStandardIsUTC ? DateTime.UtcNow : DateTime.Now);
                });
                Thread.Sleep(1);
            }
        }

        void updateDateAndTime(DateTime dt)
        {
            formattedTime.Text = dt.ToString($"HH:mm:ss  {timeLabel}");
            ISOformatted.Text = dt.ToString("yyyy-MM-dd HH:mm:ss");
            formattedDate.Text = dt.ToString("MMMM dd, yyyy");
            dayOfWeek.Text = dt.ToString("dddd");
        }
        
    }
}
