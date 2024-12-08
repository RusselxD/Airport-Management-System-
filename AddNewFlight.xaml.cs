using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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

        DoubleAnimation showAirlineChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        private HomePageControl homePage;

        public AddNewFlight(HomePageControl homePage)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            AddTerminals();
            AddAirlines();
            Show();
            this.homePage = homePage;
        }

        Dictionary<string, string> airlines = new Dictionary<string, string>
        {
            { "Aeroflot", "SU" },
            { "Air Canada", "AC" },
            { "Air France", "AF" },
            { "Air India", "AI" },
            { "Air New Zealand", "NZ" },
            { "All Nippon Airways", "NH" },
            { "American Airlines", "AA" },
            { "Austrian Airlines", "OS" },
            { "British Airways", "BA" },
            { "Cathay Pacific", "CX" },
            { "Delta Air Lines", "DL" },
            { "El Al", "LY" },
            { "Emirates", "EK" },
            { "Ethiopian Airlines", "ET" },
            { "Etihad Airways", "EY" },
            { "Finnair", "AY" },
            { "Iberia", "IB" },
            { "Japan Airlines", "JL" },
            { "KLM", "KL" },
            { "Korean Air", "KE" },
            { "Lufthansa", "LH" },
            { "Malaysia Airlines", "MH" },
            { "Philippine Airlines", "PR" },
            { "Qantas", "QF" },
            { "Qatar Airways", "QR" },
            { "Singapore Airlines", "SQ" },
            { "South African Airways", "SA" },
            { "Turkish Airlines", "TK" },
            { "United Airlines", "UA" },
            { "Virgin Atlantic", "VS" }
        };

        private void AddAirlines()
        {
            Style style = new Style(typeof(TextBlock));
            Trigger trigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            trigger.Setters.Add(new Setter(TextBlock.BackgroundProperty, new SolidColorBrush(Color.FromRgb(232, 232, 232))));
            style.Triggers.Add(trigger);

            foreach (string s in airlines.Keys)
            {
                TextBlock tb = new TextBlock()
                {
                    FontFamily = new FontFamily("Ubuntu"),
                    FontSize = 14,
                    Padding = new Thickness(10, 9, 0, 9),
                    Cursor = Cursors.Hand,
                    Text = s
                };

                tb.Style = style;
                tb.MouseDown += Choose_Airline;
                airlinesChoicesStack.Children.Add(tb);
            }
        }

        private bool airlineChoicesIsOpen = false;

        private string chosenAirline;

        private void Open_Airline_Choices(object sender, MouseButtonEventArgs e)
        {
            if (airlineChoicesIsOpen)
            {
                showAirlineChoices.From = 105;
                showAirlineChoices.To = 0;
            }
            else
            {
                showAirlineChoices.From = 0;
                showAirlineChoices.To = 105;
                flightNumberBorder.CornerRadius = new CornerRadius(8, 8, 8, 0);
            }

            airlineChoicesIsOpen = !airlineChoicesIsOpen;

            showAirlineChoices.Completed += (s, args) =>
            {
                if (!airlineChoicesIsOpen)
                {
                    flightNumberBorder.CornerRadius = new CornerRadius(8);
                }
            };

            airlineChoices.BeginAnimation(Border.HeightProperty, showAirlineChoices);
        }

        private void Choose_Airline(object sender, MouseButtonEventArgs e)
        {
            string airlineName = (sender as TextBlock).Text;
            chosenAirline = airlineName;
            airlineIATACode.Text = airlines[airlineName];

            showAirlineChoices.From = 105;
            showAirlineChoices.To = 0;

            airlineChoicesIsOpen = !airlineChoicesIsOpen;

            airlineChoices.BeginAnimation(Border.HeightProperty, showAirlineChoices);
        }

        private void AddTerminals()
        {
            string[] endPointTerminals = new string[]
            {
                "Abu Dhabi (AUH)",
                "Addis Ababa (ADD)",
                "Amsterdam (AMS)",
                "Auckland (AKL)",
                "Cape Town (CPT)",
                "Chicago (ORD)",
                "Dallas (DFW)",
                "Delhi (DEL)",
                "Doha (DOH)",
                "Dubai (DXB)",
                "Frankfurt (FRA)",
                "Helsinki (HEL)",
                "Hong Kong (HKG)",
                "Istanbul (IST)",
                "Johannesburg (JNB)",
                "Kuala Lumpur (KUL)",
                "London (LHR)",
                "Los Angeles (LAX)",
                "Madrid (MAD)",
                "Manchester (MAN)",
                "Manila (MNL)",
                "Moscow (SVO)",
                "New York (JFK)",
                "Osaka (KIX)",
                "Paris (CDG)",
                "San Francisco (SFO)",
                "Seoul (ICN)",
                "Singapore (SIN)",
                "Sydney (SYD)",
                "Tel Aviv (TLV)",
                "Tokyo (NRT)",
                "Toronto (YYZ)",
                "Vienna (VIE)"
            };

            Style style = new Style(typeof(TextBlock));
            Trigger trigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            trigger.Setters.Add(new Setter(TextBlock.BackgroundProperty, new SolidColorBrush(Color.FromRgb(232, 232, 232))));
            style.Triggers.Add(trigger);

            foreach (string s in endPointTerminals)
            {
                TextBlock tb = new TextBlock()
                {
                    FontFamily = new FontFamily("Ubuntu"),
                    FontSize = 14,
                    Padding = new Thickness(10, 9, 0, 9),
                    Cursor = Cursors.Hand,
                    Text = s
                };

                tb.Style = style;

                tb.MouseDown += Choose_EndPoint;

                endPointChoicesStack.Children.Add(tb);
            }
        }

        DoubleAnimation showEndPointChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        private bool endPointChoicesIsOpen = false;

        private void Choose_EndPoint(object sender, MouseButtonEventArgs e)
        {
            endPoint.Text = (sender as TextBlock).Text;

            showEndPointChoices.From = 105;
            showEndPointChoices.To = 0;

            endPointChoicesIsOpen = !endPointChoicesIsOpen;

            endPointChoices.BeginAnimation(Border.HeightProperty, showEndPointChoices);
        }

        private void Open_EndPoint_Choices(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (endPointChoicesIsOpen)
            {
                showEndPointChoices.From = 105;
                showEndPointChoices.To = 0;
            }
            else
            {
                showEndPointChoices.From = 0;
                showEndPointChoices.To = 105;
                b.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }

            endPointChoicesIsOpen = !endPointChoicesIsOpen;

            showEndPointChoices.Completed += (s, args) =>
            {
                if (!endPointChoicesIsOpen)
                {
                    b.CornerRadius = new CornerRadius(8);
                }
            };

            endPointChoices.BeginAnimation(Border.HeightProperty, showEndPointChoices);
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
            string gateChosen = (sender as TextBlock).Text;
            gate.Text = gateChosen;
            terminal.Text = gateChosen[0].ToString();

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

        private void Add_New_Flight(object sender, MouseButtonEventArgs e)
        {
            if (!Input_Is_Valid())
            {
                return;
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

            string table = departure.IsChecked == true ? "departure_flights" : "arrival_flights";

            string insertQuery = $"INSERT INTO {table} VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8)";

            // Values to insert
            string val1 = $"{airlineIATACode.Text}{flightNumber.Text}";
            string val2 = endPoint.Text;
            string val3 = $"{hour.Text}:{minute.Text}";
            string val4 = status.Text;
            string val5 = gate.Text;
            string val6 = terminal.Text;
            int val7 = gatesMap[val5];
            string val8 = chosenAirline;

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
                    command.Parameters.AddWithValue("@Value8", val8);

                    int success = command.ExecuteNonQuery();
                    if (success == 1)
                    {
                        homePage.Refresh_Flight_Control();
                        string a = departure.IsChecked == true ? "Departures" : "Arrivals";
                        homePage.addRecentAct($"Added Flilght {val1} to {a}.");

                        MessageBox.Show("Flight successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        airlineIATACode.Text = "";
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}