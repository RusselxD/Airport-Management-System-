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

        private SolidColorBrush darkGreenColor;
        private SolidColorBrush lighterGreenColor;

        private StackPanel departurePanel;
        private StackPanel arrivalPanel;

        private bool departureIsShown;

        public FlightsControl()
        {
            // TODO: yung kapag nag palit from departure to arrival or vice versa, yung size ng inner at outer margins i-resize


            InitializeComponent();

            departureIsShown = true;

            Initialize_Reusable_Elements();

            departureButton.Background = darkGreenColor;
            arrivalButton.Background = lighterGreenColor;
            arrivalButton.Cursor = Cursors.Hand;

            endpoint.Text = "Destination";
            flightsGrid.Children.Add(departurePanel);

            Task.Run(() => QueryDepartureFlights(MainWindow.cts.Token));
            Task.Run(() => QueryArrivalFlights(MainWindow.cts.Token));
        }

        private void Initialize_Reusable_Elements()
        {
            departurePanel = new StackPanel()
            {
                Name = "departurePanel",
                VerticalAlignment = VerticalAlignment.Top
            };
            Grid.SetRow(departurePanel, 1);

            arrivalPanel = new StackPanel()
            {
                Name = "arrivalPanel",
                VerticalAlignment = VerticalAlignment.Top
            };
            Grid.SetRow(arrivalPanel, 1);

            darkGreenColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF06611E"));
            lighterGreenColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90FDAD"));

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

        private int departureFlights = 0;

        private async Task QueryDepartureFlights(CancellationToken token)
        {
            SqlDataReader departureReader = null;
            int l1;

            while (!token.IsCancellationRequested)
            {
                l1 = 0;

                SqlCommand flightsQuery = new SqlCommand("SELECT * FROM departure_flights", MainWindow.sqlConnection);

                using (departureReader = await flightsQuery.ExecuteReaderAsync(token))
                {
                    while (departureReader.Read())
                    {
                        l1++;

                        if (l1 > departureFlights)
                        {
                            departureFlights = l1;

                            string flight = departureReader[0].ToString();
                            string dest = departureReader[1].ToString();
                            string time = departureReader[2].ToString();
                            string status = departureReader[3].ToString();
                            string gate = departureReader[4].ToString();
                            int terminal = Convert.ToInt16(departureReader[5]);

                            await Dispatcher.InvokeAsync(() =>
                            {
                                Add_New_Flight(Create_New_Row(flight, dest, time, status, gate, terminal), departurePanel);
                                if(departureIsShown && departureFlights > 8)
                                {
                                    flightsBorder.Height += 55;
                                    outerMostGrid.Height += 55;
                                }
                            });

                        }


                    }
                }

                if (token.IsCancellationRequested)
                    break;

                await Task.Delay(5, token);
            }

            departureReader?.Dispose();
            departureReader?.Close();
        }

        private async Task QueryArrivalFlights(CancellationToken token)
        {
            SqlDataReader arrivalReader = null;
            int l1;

            while (!token.IsCancellationRequested)
            {
                l1 = 0;

                SqlCommand flightsQuery = new SqlCommand("SELECT * FROM arrival_flights", MainWindow.sqlConnection);

                using (arrivalReader = await flightsQuery.ExecuteReaderAsync(token))
                {
                    while (arrivalReader.Read())
                    {
                        l1++;

                        if (l1 > arrivalFlights)
                        {
                            arrivalFlights = l1;

                            string flight = arrivalReader[0].ToString();
                            string dest = arrivalReader[1].ToString();
                            string time = arrivalReader[2].ToString();
                            string status = arrivalReader[3].ToString();
                            string gate = arrivalReader[4].ToString();
                            int terminal = Convert.ToInt16(arrivalReader[5]);

                            await Dispatcher.InvokeAsync(() =>
                            {
                                Add_New_Flight(Create_New_Row(flight, dest, time, status, gate, terminal), arrivalPanel);
                                if (!departureIsShown && arrivalFlights > 8)
                                {
                                    flightsBorder.Height += 55;
                                    outerMostGrid.Height += 55;
                                }
                            });

                        }

                    }
                }

                if (token.IsCancellationRequested)
                    break;

                await Task.Delay(5, token);
            }

            arrivalReader?.Dispose();
            arrivalReader?.Close();
        }

        private int arrivalFlights = 0;

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
            }
            grid.Children.Add(statusTb);

            grid.Children.Add(createTextBlocks($"{gate}", 0, 17, 4));
            grid.Children.Add(createTextBlocks($"{terminal}", 0, 17, 5));

            border.Child = grid;

            return border;
        }

        private void Add_New_Flight(Border newFlight, StackPanel flightsPanel)
        {
            flightsPanel.Children.Add(newFlight);

            newFlight.BeginAnimation(Border.HeightProperty, heightAnimation);
            newFlight.BeginAnimation(Border.OpacityProperty, opacityAnimation);

            //i++;
            //if (i > 9)
            //{
            //    flightsBorder.Height += 55;
            //    outerMostGrid.Height += 55;
            //}
        }

       // private int i = 1;

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

            if(departurePanel.Parent != null)
            {
                ((Grid)departurePanel.Parent).Children.Remove(departurePanel);
            }

            endpoint.Text = "Destination";
            flightsGrid.Children.Remove(arrivalPanel);
            flightsGrid.Children.Add(departurePanel);

           // if (departureFlights < 9 && arrivalFlights < 9) return;

            



                //if (departureFlights > arrivalFlights)
                //{
                //    int nigga = departureFlights - 8;
                //    int l = nigga - arrivalFlights;

                //    flightsBorder.Height += (l * 55);
                //    outerMostGrid.Height += (l * 55);
                //} 
                //else if (departureFlights < arrivalFlights)
                //{
                //    int nigga = arrivalFlights - 8;
                //    int l = nigga - departureFlights;

                //    flightsBorder.Height -= (l * 55);
                //    outerMostGrid.Height -= (l * 55);
                //}

                departureIsShown = true;

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

            if (arrivalPanel.Parent != null)
            {
                ((Grid)arrivalPanel.Parent).Children.Remove(arrivalPanel);
            }

            endpoint.Text = "Origin";
            flightsGrid.Children.Remove(departurePanel);
            flightsGrid.Children.Add(arrivalPanel);

            //if (arrivalFlights > departureFlights)
            //{
            //    int nigga = arrivalFlights - 8;

            //    int l = nigga - departureFlights;

            //    flightsBorder.Height += (l * 55);
            //    outerMostGrid.Height += (l * 55);
            //}
            //else if (arrivalFlights < departureFlights)
            //{
            //    int nigga = departureFlights - 8;
            //    int l = nigga - arrivalFlights;

            //    flightsBorder.Height -= (l * 55);
            //    outerMostGrid.Height -= (l * 55);
            //}

            departureIsShown = false;
        }
    }
}
