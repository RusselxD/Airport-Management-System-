using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for FlightsControl.xaml
    /// </summary>
    public partial class FlightsControl : UserControl
    {
        private SolidColorBrush darkGreenColor;
        private SolidColorBrush lighterGreenColor;

        private bool departureIsShown;

        List<List<string>> departures;
        List<List<string>> arrivals;

        public FlightsControl()
        {
            InitializeComponent();

            departureIsShown = true;

            Initialize_Reusable_Elements();

            departureButton.Background = darkGreenColor;
            arrivalButton.Background = lighterGreenColor;
            arrivalButton.Cursor = Cursors.Hand;

            endpoint.Text = "Destination";
            Refresh();
        }

        public async void Refresh()
        {
            departures = new List<List<string>>();
            arrivals = new List<List<string>>();

            await Task.Run(() => QueryDepartureFlights(MainWindow.cts.Token));
            await Task.Run(() => QueryArrivalFlights(MainWindow.cts.Token));

            UpdateDisplayedFlights();
        }

        private async Task QueryDepartureFlights(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    using (SqlCommand flightsQuery = new SqlCommand("SELECT * FROM departure_flights", MainWindow.sqlConnection))
                    using (SqlDataReader departureReader = await flightsQuery.ExecuteReaderAsync(token))
                    {
                        while (await departureReader.ReadAsync(token))
                        {
                            List<string> flight = new List<string>();
                            flight.Add(departureReader[1].ToString());
                            flight.Add(departureReader[2].ToString());
                            flight.Add(departureReader[3].ToString());
                            flight.Add(departureReader[4].ToString());
                            flight.Add(departureReader[5].ToString());
                            flight.Add(departureReader[6].ToString());

                            await Dispatcher.InvokeAsync(() =>
                            {
                                departures.Add(flight);
                            });
                        }
                    };
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task QueryArrivalFlights(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    using (SqlCommand flightsQuery = new SqlCommand("SELECT * FROM arrival_flights", MainWindow.sqlConnection))
                    using (SqlDataReader arrivalReader = await flightsQuery.ExecuteReaderAsync(token))
                    {
                        while (await arrivalReader.ReadAsync(token))
                        {
                            List<string> flight = new List<string>();
                            flight.Add(arrivalReader[1].ToString());
                            flight.Add(arrivalReader[2].ToString());
                            flight.Add(arrivalReader[3].ToString());
                            flight.Add(arrivalReader[4].ToString());
                            flight.Add(arrivalReader[5].ToString());
                            flight.Add(arrivalReader[6].ToString());
                            await Dispatcher.InvokeAsync(() =>
                            {
                                arrivals.Add(flight);
                            });
                        }
                    };
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void UpdateDisplayedFlights()
        {
            flightsBorder.Height = 498;
            outerMostGrid.Height = 741;
            flightsPanel.Children.Clear();

            int i = 0;
            List<List<string>> flightsToAdd;

            flightsToAdd = departureIsShown ? departures : arrivals;

            foreach (List<string> flight in flightsToAdd)
            {
                i++;
                if (i > 8)
                {
                    flightsBorder.Height += 55;
                    outerMostGrid.Height += 55;
                }

                flightsPanel.Children.Add(Create_New_Row(flight[0], flight[1], flight[2], flight[3], flight[4], flight[5]));
            }
        }

        private void Initialize_Reusable_Elements()
        {
            darkGreenColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF06611E"));
            lighterGreenColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90FDAD"));
        }

        private async void Search_Icon_Click(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                MessageBox.Show("Please enter a flight number to search for.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<string> details = new List<string>();

            try
            {
                await Task.Run(async () =>
                {
                    string flight = "";
                    await Dispatcher.InvokeAsync(() =>
                    {
                         flight = searchBox.Text.ToUpper();
                    });

                    using (SqlCommand findFlightQuery = new SqlCommand($"SELECT * FROM departure_flights WHERE flight = @Flight", MainWindow.sqlConnection))
                    {
                        findFlightQuery.Parameters.AddWithValue("@Flight", flight);
                        using (SqlDataReader flightReader = await findFlightQuery.ExecuteReaderAsync())
                        {
                            while (await flightReader.ReadAsync())
                            {
                                details.Add("departure");
                                details.Add(flightReader[1].ToString());
                                details.Add(flightReader[2].ToString());
                                details.Add(flightReader[3].ToString());
                                details.Add(flightReader[4].ToString());
                                details.Add(flightReader[5].ToString());
                                details.Add(flightReader[6].ToString());
                                details.Add(flightReader[8].ToString());
                            }
                        }
                    }
                    

                    using (SqlCommand findFlightQuery = new SqlCommand($"SELECT * FROM arrival_flights WHERE flight = @Flight", MainWindow.sqlConnection))
                    {
                        findFlightQuery.Parameters.AddWithValue("@Flight", flight);
                        using (SqlDataReader flightReader = await findFlightQuery.ExecuteReaderAsync())
                        {
                            while (await flightReader.ReadAsync())
                            {
                                details.Add("arrival");
                                details.Add(flightReader[1].ToString());
                                details.Add(flightReader[2].ToString());
                                details.Add(flightReader[3].ToString());
                                details.Add(flightReader[4].ToString());
                                details.Add(flightReader[5].ToString());
                                details.Add(flightReader[6].ToString());
                                details.Add(flightReader[8].ToString());
                            }
                        }
                    }
                    
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if(details.Count == 0)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("No Flights Found.", "No Result", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            } 
            else
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Open_Flight_Details(details);
                });
            }
        }

        private void Open_Flight_Details(List<string> details)
        {
            new FlightDetailsWindow(details);
        }

        private Grid Create_New_Grid()
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.80, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.65, GridUnitType.Star) });

            return grid;
        }

        private TextBlock createTextBlocks(string t, int p1, int p2, int colNum)
        {
            TextBlock tb = new TextBlock
            {
                Text = t,
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(p1, p2, 0, 0)
            };
            Grid.SetColumn(tb, colNum);

            return tb;
        }

        private Border Create_New_Row(string flight, string dest, string time, string status, string gate, string terminal)
        {
            Border border = new Border
            {
                Height = 55,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = Brushes.Black
            };

            Grid grid = Create_New_Grid();

            grid.Children.Add(createTextBlocks($"{flight}", 20, 17, 0));
            grid.Children.Add(createTextBlocks($"{dest}", 0, 17, 1));
            grid.Children.Add(createTextBlocks($"{time}", 0, 17, 2));

            TextBlock statusTb = createTextBlocks($"{status}", 0, 17, 3);
            switch (status)
            {
                case "On Time":
                    statusTb.Foreground = Brushes.Green;
                    break;
                case "Boarding":
                    statusTb.Foreground = Brushes.Blue;
                    break;
                case "Delayed":
                    statusTb.Foreground = Brushes.Red;
                    break;
                case "Landed":
                    statusTb.Foreground = Brushes.Gray;
                    break;
            }
            grid.Children.Add(statusTb);

            grid.Children.Add(createTextBlocks($"{gate}", 0, 17, 4));
            grid.Children.Add(createTextBlocks($"{terminal}", 0, 17, 5));

            border.Child = grid;

            return border;
        }

        private void Departure_Button_Click(object sender, MouseButtonEventArgs e)
        {
            departureButton.Background = darkGreenColor;
            departureText.Foreground = Brushes.White;

            departureButton.Cursor = Cursors.Arrow;
            departureButton.MouseDown -= Departure_Button_Click;

            arrivalButton.Cursor = Cursors.Hand;
            arrivalButton.Background = lighterGreenColor;
            arrivalButton.MouseDown += Arrival_Button_CLick;
            arrivalText.Foreground = Brushes.Black;

            endpoint.Text = "Destination";
            departureIsShown = true;
            UpdateDisplayedFlights();
        }

        private void Arrival_Button_CLick(object sender, MouseButtonEventArgs e)
        {
            arrivalButton.Background = darkGreenColor;
            arrivalText.Foreground = Brushes.White;

            arrivalButton.Cursor = Cursors.Arrow;
            arrivalButton.MouseDown -= Arrival_Button_CLick;

            departureButton.Cursor = Cursors.Hand;
            departureButton.Background = lighterGreenColor;
            departureButton.MouseDown += Departure_Button_Click;
            departureText.Foreground = Brushes.Black;

            endpoint.Text = "Origin";

            departureIsShown = false;
            UpdateDisplayedFlights();
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!searchBoxBorder.IsMouseOver)
            {
                Keyboard.ClearFocus();
            }
        }

    }
}
