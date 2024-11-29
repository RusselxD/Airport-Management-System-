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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for ReportControl.xaml
    /// </summary>
    public partial class ReportControl : UserControl
    {

        public ReportControl(SqlConnection sqlConnection)
        {
            InitializeComponent();

            terminal1Bar.Value = 50;
            terminal2Bar.Value = 40;
            terminal3Bar.Value = 90;

            Task.Run(() => QueryStats(MainWindow.cts.Token));

        }

        string[] flightsDataQueries = 
        {
            "SELECT COUNT(1) FROM arrival_flights",
            "SELECT COUNT(1) FROM departure_flights",
            "SELECT (SELECT COUNT(1) FROM departure_flights WHERE flight_status = 'On Time') + (SELECT COUNT(1) FROM arrival_flights WHERE flight_status = 'On Time');",
            "SELECT (SELECT COUNT(1) FROM departure_flights WHERE flight_status = 'Delayed') + (SELECT COUNT(1) FROM arrival_flights WHERE flight_status = 'Delayed');",
            "SELECT COUNT(1) FROM arrival_flights WHERE flight_status = 'Landed';"
        };

        int[] vvvv = new int[5];
        // arrivals
        // deps
        // on time
        // delay
        // landed

        private async Task QueryStats(CancellationToken token)
        {
            try
            {
                for(int i = 0; i < 5; i++)
                {
                    SqlDataReader countReader = null;
                    SqlCommand countQuery = new SqlCommand(flightsDataQueries[i], MainWindow.sqlConnection);

                    using (countReader = await countQuery.ExecuteReaderAsync(token))
                    {
                        while (await countReader.ReadAsync())
                        {
                            vvvv[i] = Convert.ToInt32(countReader[0]);
                        }
                    }
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    arrivals.Text = vvvv[0].ToString();
                    departures.Text = vvvv[1].ToString();
                    delayed.Text = vvvv[3].ToString();
                    landed.Text = vvvv[4].ToString();


                    int t = vvvv[0] + vvvv[1];
                    totalFlights.Text = t.ToString();

                    double p = (double)vvvv[2] / t * 100;
                    onTime.Text = $"{p.ToString("F2")}%";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
