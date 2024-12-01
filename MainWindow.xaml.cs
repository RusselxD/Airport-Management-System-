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
        public static SqlConnection sqlConnection;

        private HomePageControl homePageControl;
        private FlightsControl flightsControl;
        private ServicesControl servicesControl;
        private GatesControl gatesControl;
        private StaffControl staffControl;
        private ReportControl reportControl;

        private bool appIsRunning;

        /*
         * 0 - Home
         * 1 - Flights
         * 2 - Services
         * 3 - Gates
         * 4 - Staff
         * 5 - Report
         */
        private int currentControl;
        private SolidColorBrush currentNavBackgroundColor;

        private bool timeStandardIsUTC;
        private string timeLabel;

        public static CancellationTokenSource cts;

        public MainWindow()
        {
            appIsRunning = true;

            // NOTE: Connection only temporary.
            string connectionstring = @"server=DESKTOP-4CVBSIM\SQLEXPRESS; database=airport_database;user id=airport_admin;password=admin;MultipleActiveResultSets = True;";
            sqlConnection = new SqlConnection(connectionstring);
            sqlConnection.Open();
            
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //this.sqlConnection = sqlConnection;

            cts = new CancellationTokenSource();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                DisplayDateAndTime();
            }).Start();
            timeStandardIsUTC = true;
            timeLabel = "UTC";

            homePageControl = new HomePageControl(this);
            flightsControl = new FlightsControl();
            servicesControl = new ServicesControl(sqlConnection);
            gatesControl = new GatesControl();
            staffControl = new StaffControl(sqlConnection);
            reportControl = new ReportControl(sqlConnection);

            CurrentPage.Content = homePageControl;
            currentControl = 0;

            InitializeNavLabelsAndProperties();

            Show();
            this.Closed += (s, e) => closeApp();
        }

        Style navLabelStyle;

        void InitializeNavLabelsAndProperties()
        {
            // Grayish background color for label that represent the current page (User Control)
            currentNavBackgroundColor = new SolidColorBrush(Color.FromRgb(246, 246, 246));

            // ----------------- LABEL HOVER EFFECT ----------------- //
            navLabelStyle = new Style(typeof(Label));
            Trigger mouseOverLabel = new Trigger
            {
                Property = Label.IsMouseOverProperty,
                Value = true
            };
            mouseOverLabel.Setters.Add(new Setter(Label.BackgroundProperty, new SolidColorBrush(Color.FromRgb(233, 233, 233))));
            navLabelStyle.Triggers.Add(mouseOverLabel);
            // ------------------------------------------------------ //

            // -------------------- NAVIGATION LABELS -------------------- //
            homeNavLabel.Background = currentNavBackgroundColor;

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
            // ----------------------------------------------------------- //
        }

        void closeApp()
        {
            this.appIsRunning = false;
            cts.Cancel();
            //   homePageControl.CloseConnection();
            sqlConnection.Close();
            sqlConnection.Dispose();
        }

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
                }).Task.Wait();

                if (!appIsRunning)
                    break;
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

        public void changeCurrentControl(int i)
        {
            if (i == currentControl)
                return;

            // Current Nav Label
            RedesignNavLabel();
            currentControl = i;

            // New Nav Label
            Label navLabel = default;
            switch (i)
            {
                case 0:
                    CurrentPage.Content = homePageControl;
                    navLabel = homeNavLabel;
                    break;

                case 1:
                    CurrentPage.Content = flightsControl;
                    navLabel = flightsNavLabel;
                    break;

                case 2:
                    CurrentPage.Content = servicesControl;
                    navLabel = servicesNavLabel;
                    break;

                case 3:
                    CurrentPage.Content = gatesControl;
                    navLabel = gatesNavLabel;
                    break;

                case 4:
                    CurrentPage.Content = staffControl;
                    navLabel = staffNavLabel;
                    break;

                case 5:
                    CurrentPage.Content = reportControl;
                    navLabel = reportNavLabel;
                    break;
            }

            navLabel.Cursor = null;
            navLabel.Background = currentNavBackgroundColor;
            navLabel.Style = null;
        }

        /*
         * These will be applied to the current label
         * 2. Remove the Background
         * 1. Apply the Hand Cursor
         * 3. Re-Apply the hover effect (navLabelStyle)
         */
        private void RedesignNavLabel()
        {
            Label currentLabel = default;

            switch (currentControl)
            {
                case 0:
                    currentLabel = homeNavLabel;
                    break;
                case 1:
                    currentLabel = flightsNavLabel;
                    break;
                case 2:
                    currentLabel = servicesNavLabel;
                    break;
                case 3:
                    currentLabel = gatesNavLabel;
                    break;
                case 4:
                    currentLabel = staffNavLabel;
                    break;
                case 5:
                    currentLabel = reportNavLabel;
                    break;
            }

            currentLabel.Background = null;
            currentLabel.Cursor = Cursors.Hand;
            currentLabel.ClearValue(Label.BackgroundProperty);
            currentLabel.Style = navLabelStyle;
        }

        private void Nav_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label clickedNavLabel)
            {
                string labelName = clickedNavLabel.Name;

                switch (labelName)
                {
                    case "homeNavLabel":
                        changeCurrentControl(0);
                        break;
                    case "flightsNavLabel":
                        changeCurrentControl(1);
                        break;
                    case "servicesNavLabel":
                        changeCurrentControl(2);
                        break;
                    case "gatesNavLabel":
                        changeCurrentControl(3);
                        break;
                    case "staffNavLabel":
                        changeCurrentControl(4);
                        break;
                    case "reportNavLabel":
                        changeCurrentControl(5);
                        break;
                }
            }
        }

        public void Refresh_Flight_And_Gate_Controls()
        {
            flightsControl.Refresh();
            gatesControl.Refresh();
        }
    }
}
