using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for AssignGate.xaml
    /// </summary>
    public partial class AssignGate : Window
    {
        private HomePageControl homePage;
        public AssignGate(HomePageControl homePage)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Task.Run(() => Query_Gates(MainWindow.cts.Token));
            Task.Run(() => Query_Flights(MainWindow.cts.Token));
            this.homePage = homePage;
            Show();
        }

        Border chosenFlight;

       

        Border chosenGate;

        private async Task Query_Gates(CancellationToken token)
        {
            try
            {
                List<(string GateName, int status)> gates = new List<(string, int)>();
                List<(string gateName, string gateDetails)> occupiedGatesList = new List<(string, string)>();


                await Task.Run(async () =>
                {
                    using (SqlCommand gatesQuery = new SqlCommand("SELECT * FROM gates_table", MainWindow.sqlConnection))
                    using (SqlDataReader gatesReader = await gatesQuery.ExecuteReaderAsync(token))
                    {
                        while (await gatesReader.ReadAsync())
                        {
                            gates.Add((gatesReader[1].ToString(), Convert.ToInt16(gatesReader[2])));
                            if (Convert.ToInt16(gatesReader[2]) == 3)
                            {
                                occupiedGatesList.Add((gatesReader[1].ToString(), gatesReader[3].ToString()));
                            }
                        }
                    } 
                }, token);

                await Dispatcher.InvokeAsync(() =>
                {
                    int row = 0;
                    int left = 20;
                    int bot = 10;
                    int i = 0;

                    foreach (var g in gates)
                    {
                        if (i % 2 == 0)
                        {
                            row++;
                            left -= 5;
                            bot += 5;
                        }

                        Border b = new Border()
                        {
                            Name = $"_{g.GateName}",
                            CornerRadius = new CornerRadius(5),
                            BorderThickness = new Thickness(1.5)
                        };

                        switch (g.status)
                        {
                            case 1:
                                b.Cursor = Cursors.Hand;
                                b.BorderBrush = new SolidColorBrush(Color.FromRgb(30, 168, 67));

                                b.MouseDown += (s, e) =>
                                {
                                    if (chosenGate != b)
                                    {
                                        if(chosenGate != null)
                                            chosenGate.Background = Brushes.White;

                                        gate.Text = g.GateName;
                                        b.Background = new SolidColorBrush(Color.FromRgb(152, 253, 179));
                                        chosenGate = b;
                                    }
                                    else
                                    {
                                        gate.Text = "";
                                        b.Background = Brushes.White;
                                        chosenGate = null;
                                    }
                                };

                                break;
                            case 2:
                                b.Background = new SolidColorBrush(Color.FromRgb(255, 215, 166));
                                b.BorderBrush = new SolidColorBrush(Color.FromRgb(248, 142, 12));
                                break;
                            case 3:
                                b.Background = new SolidColorBrush(Color.FromRgb(255, 192, 192));
                                b.BorderBrush = new SolidColorBrush(Color.FromRgb(247, 28, 28));
                                break;
                        }

                        if (i % 2 == 0)
                        {
                            b.Margin = new Thickness(10, left, 5, bot);
                        }
                        else
                        {
                            b.Margin = new Thickness(5, left, 10, bot);
                        }

                        Grid.SetColumn(b, i % 2);
                        Grid.SetRow(b, row);

                        TextBlock tb = new TextBlock()
                        {
                            Text = $"{g.GateName}",
                            FontFamily = new FontFamily("Ubuntu"),
                            FontSize = 17,
                            Padding = new Thickness(22, 13.5, 0, 0),
                        };

                        b.Child = tb;

                        if (i >= 0 && i <= 5)
                        {
                            terminal1.Children.Add(b);
                        }
                        else if (i >= 6 && i <= 11)
                        {
                            terminal2.Children.Add(b);
                        }
                        else
                        {
                            terminal3.Children.Add(b);
                        }

                        if (i == 5 || i == 11)
                        {
                            row = 0;
                            left = 20;
                            bot = 10;
                        }
                        i++;
                    }
                });

                await Dispatcher.InvokeAsync(() =>
                {
                    foreach(var gate in occupiedGatesList)
                    {
                        TextBlock tb = new TextBlock()
                        {
                            Height = 34,
                            Text = $"{gate.gateName}   |   {gate.gateDetails}",
                            FontFamily = new FontFamily("Ubuntu"),
                            FontSize = 16,
                            Padding = new Thickness(15, 9, 0, 0)
                        };
                        occupiedGates.Children.Add(tb);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task Query_Flights(CancellationToken token)
        {
            var flightsList = await Task.Run( async () =>
            {
                List<(string flightNum, string assignMode)> list = new List<(string, string)>();

                using (SqlCommand command1 = new SqlCommand("SELECT * FROM departure_flights WHERE NOT flight_status = 'Boarding';", MainWindow.sqlConnection))
                using (SqlDataReader reader1 = await command1.ExecuteReaderAsync(token))
                {
                    while (reader1.Read())
                    {
                        list.Add((reader1[1].ToString(), "Boarding"));
                    }
                }

                using (SqlCommand command1 = new SqlCommand("SELECT * FROM arrival_flights WHERE NOT flight_status = 'Landed';", MainWindow.sqlConnection))
                using (SqlDataReader reader1 = await command1.ExecuteReaderAsync(token))
                {
                    while (reader1.Read())
                    {
                        list.Add((reader1[1].ToString(), "Deplaning"));
                    }
                }

                list.Sort((x, y) => x.flightNum.CompareTo(y.flightNum));

                return list;
            });

            await Dispatcher.InvokeAsync(() =>
            {
                foreach(var f in flightsList)
                {
                    Border b = new Border()
                    {
                        Name = $"{f.flightNum}",
                        Height = 45,
                        Cursor = Cursors.Hand,
                    };

                    TextBlock tb = new TextBlock()
                    {
                        Text = $"{f.flightNum} - {f.assignMode}",
                        Padding = new Thickness(10, 13, 0, 0),
                        FontFamily = new FontFamily("Ubuntu"),
                        FontSize = 16
                    };

                    b.Child = tb;

                    b.MouseEnter += (sender, e) =>
                    {
                        b.Background = new SolidColorBrush(Color.FromRgb(233, 233, 233));
                    };

                    b.MouseLeave += (sender, e) =>
                    {
                        if (b != chosenFlight)
                            b.Background = Brushes.White;
                    };

                    b.MouseDown += (s1, e1) =>
                    {
                        if (chosenFlight != b)
                        {
                            if (chosenFlight != null)
                                chosenFlight.Background = Brushes.White;

                            flight.Text = f.flightNum;
                            purpose.Text = f.assignMode;
                            dashLine.Text = "/";
                            b.Background = new SolidColorBrush(Color.FromRgb(233, 233, 233));
                            chosenFlight = b;
                        }
                        else
                        {
                            flight.Text = "";
                            purpose.Text = "";
                            dashLine.Text = "";
                            b.Background = Brushes.White;
                            chosenFlight = null;
                        }
                    };

                    flights.Children.Add(b);
                }
            });
        }

        private void Clear(object sender, MouseButtonEventArgs e)
        {
            if (chosenFlight != null)
            {
                chosenFlight.Background = Brushes.White;
                chosenFlight = null;
            }
                
            if(chosenGate != null)
            {
                chosenGate.Background = Brushes.White;
                chosenGate = null;
            }

            flight.Text = "";
            gate.Text = "";
            purpose.Text = "";
            dashLine.Text = "";
        }

        private void Click_Assign(object sender, MouseButtonEventArgs e)
        {
            if (Input_Is_Invalid()) return;

            homePage.addRecentAct($"Assigned Flight {chosenFlight.Name} to Gate {chosenGate.Name.Substring(1)}");

        }

        private bool Input_Is_Invalid()
        {
            if (chosenFlight == null)
            {
                MessageBox.Show("Choose a flight.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }

            if (chosenGate == null)
            {
                MessageBox.Show("Choose a gate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            return false;
        }
    }
}
