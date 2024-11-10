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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for FlightsControl.xaml
    /// </summary>
    public partial class FlightsControl : UserControl
    {
        private DoubleAnimation heightAnimation;
        private DoubleAnimation opacityAnimation;

        private bool departureIsShown;


        public FlightsControl(SqlConnection sqlConnection)
        {
            InitializeComponent();

            departureIsShown = true;

            Task.Run(() => QueryFlights(MainWindow.cts.Token));

            this.heightAnimation = new DoubleAnimation
            {
                From = 0,
                To = 55, // Target height, adjust as needed
                Duration = TimeSpan.FromSeconds(0.4), // Duration of the animation
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } // Smooth easing
            };

            this.opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
        }

        private string flights_table = "departure_flights";

        private int flights = 0;
        private async Task QueryFlights(CancellationToken token)
        {
            SqlDataReader flightsReader = null;
            int l1;

            while (!token.IsCancellationRequested)
            {
                l1 = 0;

                SqlCommand flightsQuery = new SqlCommand($"SELECT * FROM {flights_table}", MainWindow.sqlConnection);

                using (flightsReader = await flightsQuery.ExecuteReaderAsync(token))
                {
                    while (flightsReader.Read())
                    {
                        l1++;

                        if (l1 > flights)
                        {
                            flights = l1;

                            string flight = flightsReader[0].ToString();
                            string dest = flightsReader[1].ToString();
                            string time = flightsReader[2].ToString();
                            string status = flightsReader[3].ToString();
                            string gate = flightsReader[4].ToString();
                            int terminal = Convert.ToInt16(flightsReader[5]);


                            await Dispatcher.InvokeAsync(() =>
                            {
                                Add_New_Flight(Create_New_Row(flight, dest, time, status, gate, terminal));
                            });

                            //alerts = l1;

                            //if (alerts == 1)
                            //{
                            //    await Dispatcher.InvokeAsync(() =>
                            //    {
                            //        AlertsPanel.Children.Clear();
                            //        alertsGrid.Children.Add(deleteAllIcon);
                            //        alertsContainer.Background = greenAlertBackground;
                            //        alertsContainer.BeginAnimation(Border.OpacityProperty, alertBorderOpacityAnimation);
                            //    });
                            //}
                            //else
                            //{
                            //    await Dispatcher.InvokeAsync(() =>
                            //    {
                            //        alertsContainer.Height += 59;

                            //        alertsRow += 59;
                            //        dashboardInnerGrid.RowDefinitions[1].Height = new GridLength(alertsRow);
                            //    });
                            //}

                            //int alertID = Convert.ToInt16(alertsReader[0]);
                            //string alertMessage = alertsReader[1].ToString();
                            //int alertCode = Convert.ToInt16(alertsReader[2]);

                            //Dispatcher.InvokeAsync(() =>
                            //{
                            //    AddAlert(alertID, alertMessage, alertCode);
                            //}).Task.Wait();
                        }

                    }
                }

                if (token.IsCancellationRequested)
                    break;

                await Task.Delay(5, token);
            }

            flightsReader?.Dispose();
            flightsReader?.Close();
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(searchBox.Text == "Search Flight")
            {
                searchBox.Text = "";
                searchBox.Foreground = Brushes.Black;
            }
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.Text = "Search Flight";
                searchBox.Foreground = Brushes.Gray;
            }
        }

        private void Search_Icon_Click(object sender, MouseButtonEventArgs e)
        {
            
        }

        private Grid Create_New_Grid()
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.95, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
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

        private Border Create_New_Row(string flight, string dest, string time, string status, string gate, int terminal)
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

            TextBlock s = new TextBlock
            {
                Text = $"{status}",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(0, 17, 0, 0)
            };
            switch (status)
            {
                case "On Time":
                    s.Foreground = Brushes.Green;
                    break;
                case "Boarding":
                    s.Foreground = Brushes.Blue;
                    break;
                case "Delayed":
                    s.Foreground = Brushes.Red;
                    break;
            }

            Grid.SetColumn(s, 3);
            grid.Children.Add(s);

            grid.Children.Add(createTextBlocks($"{gate}", 0, 17, 4));
            grid.Children.Add(createTextBlocks($"{terminal}", 0, 17, 5));

            border.Child = grid;

            return border;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Add_New_Flight(Border newFlight)
        {

            flightsStackPanel.Children.Add(newFlight);

            newFlight.BeginAnimation(Border.HeightProperty, heightAnimation);
            newFlight.BeginAnimation(Border.OpacityProperty, opacityAnimation);

            i++;
            if (i > 9)
            {
                flightsBorder.Height += 55;
                outerMostGrid.Height += 55;
            }
        }

        private int i = 1;

    }
}
