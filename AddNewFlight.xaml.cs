using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for AddNewFlight.xaml
    /// </summary>
    public partial class AddNewFlight : Window
    {
        DoubleAnimation showStatusChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        DoubleAnimation showGateChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        DoubleAnimation showTerminalChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        private HomePageControl homePage;

        public AddNewFlight(HomePageControl homePage)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Show();
            this.homePage = homePage;
        }

        private void Choose_Status(object sender, MouseButtonEventArgs e)
        {
            status.Text = (sender as TextBlock).Text;

            showStatusChoices.From = 70;
            showStatusChoices.To = 0;

            statusChoicesIsOpen = !statusChoicesIsOpen;

            statusChoices.BeginAnimation(Border.HeightProperty, showStatusChoices);
        }

        private bool statusChoicesIsOpen = false;

        private void Open_Status_Choices(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (statusChoicesIsOpen)
            {
                showStatusChoices.From = 70;
                showStatusChoices.To = 0;
            }
            else
            {
                showStatusChoices.From = 0;
                showStatusChoices.To = 70;
                b.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }

            statusChoicesIsOpen = !statusChoicesIsOpen;

            showStatusChoices.Completed += (s, args) =>
            {
                if (!statusChoicesIsOpen)
                {
                    b.CornerRadius = new CornerRadius(8);
                }
            };

            statusChoices.BeginAnimation(Border.HeightProperty, showStatusChoices);
        }

        private void Choose_Gate(object sender, MouseButtonEventArgs e)
        {
            gate.Text = (sender as TextBlock).Text;

            showGateChoices.From = 70;
            showGateChoices.To = 0;

            gateChoicesIsOpen = !gateChoicesIsOpen;

            gateChoices.BeginAnimation(Border.HeightProperty, showGateChoices);
        }

        private bool gateChoicesIsOpen = false;

        private void Open_Gate_Choices(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (gateChoicesIsOpen)
            {
                showGateChoices.From = 70;
                showGateChoices.To = 0;
            }
            else
            {
                showGateChoices.From = 0;
                showGateChoices.To = 70;
                b.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }

            gateChoicesIsOpen = !gateChoicesIsOpen;

            showGateChoices.Completed += (s, args) =>
            {
                if (!gateChoicesIsOpen)
                {
                    b.CornerRadius = new CornerRadius(8);
                }
            };

            gateChoices.BeginAnimation(Border.HeightProperty, showGateChoices);
        }

        private void Choose_Terminal(object sender, MouseButtonEventArgs e)
        {
            terminal.Text = (sender as TextBlock).Text;

            showTerminalChoices.From = 70;
            showTerminalChoices.To = 0;

            terminalChoicesIsOpen = !terminalChoicesIsOpen;

            terminalChoices.BeginAnimation(Border.HeightProperty, showTerminalChoices);
        }

        private bool terminalChoicesIsOpen = false;

        private void Open_Terminal_Choices(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (terminalChoicesIsOpen)
            {
                showTerminalChoices.From = 70;
                showTerminalChoices.To = 0;
            }
            else
            {
                showTerminalChoices.From = 0;
                showTerminalChoices.To = 70;
                b.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }

            terminalChoicesIsOpen = !terminalChoicesIsOpen;

            showTerminalChoices.Completed += (s, args) =>
            {
                if (!terminalChoicesIsOpen)
                {
                    b.CornerRadius = new CornerRadius(8);
                }
            };


            terminalChoices.BeginAnimation(Border.HeightProperty, showTerminalChoices);
        }

        private void Change_EndPoint(object sender, RoutedEventArgs e)
        {
            string t = (sender as RadioButton).Content.ToString();

            if (t.Equals("Departure"))
            {
                endPointLabel.Text = "Destination";
            }
            else
            {
                endPointLabel.Text = "Origin";
            }
        }

        private bool Input_Is_Valid()
        {
            if (flightNumber.Text == "")
            {
                MessageBox.Show("Please enter a flight number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (endPoint.Text == "")
            {
                MessageBox.Show("Please enter a destination.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (hour.Text == "" || minute.Text == "")
            {
                MessageBox.Show("Please enter a time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (status.Text == "")
            {
                MessageBox.Show("Please choose a status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (gate.Text == "")
            {
                MessageBox.Show("Please choose a gate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (terminal.Text == "")
            {
                MessageBox.Show("Please choose a terminal.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        Dictionary<string, int> gatesMap = new Dictionary<string, int>
        {
            { "1A", 1 },
            { "1B", 2 },
            { "1C", 3 },
            { "1D", 4 },
            { "1E", 5 },
            { "1F", 6 },
            { "2A", 7 },
            { "2B", 8 },
            { "2C", 9 },
            { "2D", 10 },
            { "2E", 11 },
            { "2F", 12 },
            { "3A", 13 },
            { "3B", 14 },
            { "3C", 15 },
            { "3D", 16 },
            { "3E", 17 },
            { "3F", 18 },
        };

        private void Add_New_Flight(object sender, MouseButtonEventArgs e)
        {
            if (!Input_Is_Valid())
            {
                return;
            }

            string table = departure.IsChecked == true ? "departure_flights" : "arrival_flights";

            string insertQuery = $"INSERT INTO {table} VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7)";

            // Values to insert
            string val1 = flightNumber.Text;
            string val2 = endPoint.Text;
            string val3 = $"{hour.Text}:{minute.Text}";
            string val4 = status.Text;
            string val5 = gate.Text;
            string val6 = terminal.Text;
            int val7 = gatesMap[val5];

            try
            {
                using (SqlCommand command = new SqlCommand(insertQuery, MainWindow.sqlConnection))
                {
                    command.Parameters.AddWithValue("@Value1", val1);
                    command.Parameters.AddWithValue("@Value2", val2);
                    command.Parameters.AddWithValue("@Value3", val3);
                    command.Parameters.AddWithValue("@Value4", val4);
                    command.Parameters.AddWithValue("@Value5", val5);
                    command.Parameters.AddWithValue("@Value6", val6);
                    command.Parameters.AddWithValue("@Value7", val7);

                    int success = command.ExecuteNonQuery();
                    if(success == 1)
                    {
                        string a = departure.IsChecked == true ? "Departures" : "Arrivals";
                        homePage.addRecentAct($"Added Flilght {val1} to {a}.");

                        MessageBox.Show("Flight successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        flightNumber.Text = "";
                        endPoint.Text = "";
                        hour.Text = "";
                        minute.Text = "";
                        status.Text = "";
                        gate.Text = "";
                        terminal.Text = "";
                    }
                    
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
