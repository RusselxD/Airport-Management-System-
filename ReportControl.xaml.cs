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

            Task.Run(() => QueryStats(MainWindow.cts.Token));

        }

        string[] flightsDataQueries =
        {
            "SELECT COUNT(1) FROM arrival_flights",
            "SELECT COUNT(1) FROM departure_flights",
            "SELECT (SELECT COUNT(1) FROM departure_flights WHERE flight_status = 'On Time') + (SELECT COUNT(1) FROM arrival_flights WHERE flight_status = 'On Time');",
            "SELECT (SELECT COUNT(1) FROM departure_flights WHERE flight_status = 'Delayed') + (SELECT COUNT(1) FROM arrival_flights WHERE flight_status = 'Delayed');",
            "SELECT COUNT(1) FROM arrival_flights WHERE flight_status = 'Landed';",
            "SELECT COUNT(1) FROM gates_table WHERE gate_id <= 6 AND status_col = 3;",
            "SELECT COUNT(1) FROM gates_table WHERE gate_id BETWEEN 7 AND 12 AND status_col = 3;",
            "SELECT COUNT(1) FROM gates_table WHERE gate_id > 12 AND status_col = 3;"
        };

        int[] vvvv = new int[8];
        // arrivals
        // deps
        // on time
        // delay
        // landed
        // terminal 1 occupied
        // terminal 2 occupied
        // terminal 3 occupied

        private async Task QueryStats(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i < 8; i++)
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

                    double p = (double)vvvv[2] / (vvvv[2] + vvvv[3]) * 100;
                    onTime.Text = $"{p.ToString("#.##")}%";

                    double t1 = (double)vvvv[5] / 6 * 100;
                    double t2 = (double)vvvv[6] / 6 * 100;
                    double t3 = (double)vvvv[7] / 6 * 100;

                    terminal1Bar.Value = t1;
                    terminal2Bar.Value = t2;
                    terminal3Bar.Value = t3;

                    terminal1BarText.Text = $"{t1.ToString("#.")}%";
                    terminal2BarText.Text = $"{t2.ToString("#.")}%";
                    terminal3BarText.Text = $"{t3.ToString("#.")}%";
                });

                await Task.Delay(1000);
            }
        }
    }
}
