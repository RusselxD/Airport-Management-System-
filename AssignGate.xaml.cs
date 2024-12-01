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
        private Border chosenFlight;
        private Border chosenGate;

        public AssignGate(HomePageControl homePage)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Refresh_Window();
            this.homePage = homePage;
            Show();
        }

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
                                        if (chosenGate != null)
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
                    foreach (var gate in occupiedGatesList)
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
            var flightsList = await Task.Run(async () =>
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
                foreach (var f in flightsList)
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
            Clear_Window();
        }

        private void Clear_Window()
        {
            if (chosenFlight != null)
            {
                chosenFlight.Background = Brushes.White;
                chosenFlight = null;
            }

            if (chosenGate != null)
            {
                chosenGate.Background = Brushes.White;
                chosenGate = null;
            }

            flight.Text = "";
            gate.Text = "";
            purpose.Text = "";
            dashLine.Text = "";
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

        private async Task<bool> Update_Database(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    string updateGateQuery = "UPDATE gates_table SET status_col = 3, details_col = @DetailsValue WHERE name_col = @GateName";
                    using (SqlCommand update = new SqlCommand(updateGateQuery, MainWindow.sqlConnection))
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            update.Parameters.AddWithValue("@DetailsValue", $"{flight.Text} - {purpose.Text}");
                            update.Parameters.AddWithValue("@GateName", $"{gate.Text}");
                        });

                        await update.ExecuteNonQueryAsync(token);
                    }

                    int gateID = 0;

                    using (SqlCommand getGateID = new SqlCommand("SELECT gate_id FROM gates_table WHERE name_col = @GateValue", MainWindow.sqlConnection))
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            getGateID.Parameters.AddWithValue("@GateValue", $"{gate.Text}");
                        });
                        using (SqlDataReader getGateIDReader = await getGateID.ExecuteReaderAsync(token))
                        {
                            while (await getGateIDReader.ReadAsync())
                            {
                                gateID = Convert.ToInt16(getGateIDReader[0]);
                            }
                        }
                    }

                    string flights_table = "";
                    await Dispatcher.InvokeAsync(() =>
                    {
                        flights_table = purpose.Text.Equals("Boarding") ? "departure_flights" : "arrival_flights";
                    });

                    string updateFlightQuery = $"UPDATE {flights_table} SET flight_status = @StatusValue, gate = @GateValue, terminal = @TerminalValue, gate_id = @GateIDValue WHERE flight = @FlightValue";
                    using (SqlCommand update = new SqlCommand(updateFlightQuery, MainWindow.sqlConnection))
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            update.Parameters.AddWithValue("@StatusValue", $"{purpose.Text}");
                            update.Parameters.AddWithValue("@GateValue", $"{gate.Text}");
                            update.Parameters.AddWithValue("@TerminalValue", $"{gate.Text.ElementAt(0)}");
                            update.Parameters.AddWithValue("@GateIDValue", $"{gateID}");
                            update.Parameters.AddWithValue("@FlightValue", $"{flight.Text}");
                        });

                        await update.ExecuteNonQueryAsync(token);
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async void Click_Assign(object sender, MouseButtonEventArgs e)
        {
            if (Input_Is_Invalid()) return;

            if (await Task.Run(() => Update_Database(MainWindow.cts.Token)))
            {
                homePage.addRecentAct($"Assigned Flight {chosenFlight.Name} to Gate {gate.Text}");
                MessageBox.Show("Gate successfully assigned.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Clear_Window();
                Refresh_Window();
                homePage.Refresh_Gate_And_Flight_Controls();
            }
        }

        private async void Refresh_Window()
        {
            flights.Children.Clear();
            terminal1.Children.Clear();
            terminal2.Children.Clear();
            terminal3.Children.Clear();
            occupiedGates.Children.Clear();

            terminal1.Children.Add(Create_Terminal_Header("Terminal 1"));
            terminal2.Children.Add(Create_Terminal_Header("Terminal 2"));
            terminal3.Children.Add(Create_Terminal_Header("Terminal 3"));

            await Task.Run(() => Query_Gates(MainWindow.cts.Token));
            await Task.Run(() => Query_Flights(MainWindow.cts.Token));
        }

        private TextBlock Create_Terminal_Header(string text)
        {
            TextBlock terminalHeader = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 58,
                Width = 139,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 19,
                FontWeight = FontWeights.Medium,
                Padding = new Thickness(20, 25, 0, 0),
                Text = text
            };
            Grid.SetColumnSpan(terminalHeader, 2);
            return terminalHeader;
        }
    }
}
