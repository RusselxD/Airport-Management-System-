using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
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
        private SqlConnection sqlConnection;
        private HomePageControl homePageControl;

        private bool appIsRunning = true;

        public MainWindow()
        {
            // NOTICE: Connection only temporary.
            string connectionString = @"Server=DESKTOP-4CVBSIM\SQLEXPRESS; Database=airport_database;User Id=airport_admin;Password=admin;";
            this.sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //this.sqlConnection = sqlConnection;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                DisplayDateAndTime();
            }).Start();

            homePageControl = new HomePageControl(sqlConnection);

            CurrentPage.Content = homePageControl;

            InitializeNavLabels();
            currentControl = 0;

            Show();
            this.Closed += (s, e) => closeApp();
        }

        void InitializeNavLabels()
        {
            homeNavLabel.Background = new SolidColorBrush(Color.FromRgb(246, 246, 246));

            Style navLabelStyle = new Style(typeof(Label));
            Trigger mouseOverLabel = new Trigger
            {
                Property = Label.IsMouseOverProperty,
                Value = true
            };
            mouseOverLabel.Setters.Add(new Setter(Label.BackgroundProperty, new SolidColorBrush(Color.FromRgb(233, 233, 233))));
            navLabelStyle.Triggers.Add(mouseOverLabel);

            flightsNavLabel.Style = navLabelStyle;
            flightsNavLabel.Cursor = Cursors.Hand;

            servicesNavLabel.Style = navLabelStyle;
            servicesNavLabel.Cursor = Cursors.Hand;

            gatesNavLabel.Style = navLabelStyle;
            gatesNavLabel.Cursor = Cursors.Hand;

            staffNavLabel.Style = navLabelStyle;
            staffNavLabel.Cursor = Cursors.Hand;

            reportNavLabel.Style = navLabelStyle;
            reportNavLabel.Cursor = Cursors.Hand;

        }

        void closeApp()
        {
            appIsRunning = false;
            sqlConnection.Close();
            sqlConnection.Dispose();
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
                Dispatcher.InvokeAsync(() =>
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
        /*
         * 0 - Home
         * 1 - Flights
         * 2 - Services
         * 3 - Gates
         * 4 - Staff
         * 5 - Report
         */
        private int currentControl;
        private void changeCurrentControl(int i)
        {
            if (i == currentControl)
                return;

            // TODO : Change the current control to the one specified by i
            switch (i)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }

        }

        private void homeNavLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
