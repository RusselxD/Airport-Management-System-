using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            "SELECT COUNT(1) FROM gates_table WHERE gate_id > 12 AND status_col = 3;",

            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'On Duty';",
            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'Off Duty';",
            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'Sick';",
            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'On Leave';",

            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'On Duty' AND shift_col = 'Morning';",
            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'On Duty' AND shift_col = 'Afternoon';",
            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'On Duty' AND shift_col = 'Night';"
        };

        int[] vvvv = new int[15];
        // arrivals
        // deps
        // on time
        // delay
        // landed
        // terminal 1 occupied
        // terminal 2 occupied
        // terminal 3 occupied

        // on duty count
        // off duty count
        // sick count
        // on leave count

        // morning on duty count
        // afternoon on duty count
        // night on duty count

        private async Task QueryStats(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            for (int i = 0; i < vvvv.Length; i++)
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
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                        
                        int t = vvvv[0] + vvvv[1];

                        double p = (double)vvvv[2] / (vvvv[2] + vvvv[3]) * 100;

                        double t1 = (double)vvvv[5] / 6 * 100;
                        double t2 = (double)vvvv[6] / 6 * 100;
                        double t3 = (double)vvvv[7] / 6 * 100;

                        double onDutyValue = (double)vvvv[8] / vvvv[9] * 100;
                        double offDutyValue = (double)vvvv[9] / vvvv[9] * 100;
                        double sickValue = (double)vvvv[10] / vvvv[9] * 100;
                        double onLeaveValue = (double)vvvv[11] / vvvv[9] * 100;

                        await Dispatcher.InvokeAsync(() =>
                        {
                            arrivals.Text = vvvv[0].ToString();
                            departures.Text = vvvv[1].ToString();
                            delayed.Text = vvvv[3].ToString();
                            landed.Text = vvvv[4].ToString();

                            totalFlights.Text = t.ToString();

                            onTime.Text = $"{p.ToString("#.##")}%";

                            terminal1Bar.Value = t1;
                            terminal2Bar.Value = t2;
                            terminal3Bar.Value = t3;

                            terminal1BarText.Text = $"{t1.ToString("#.")}%";
                            terminal2BarText.Text = $"{t2.ToString("#.")}%";
                            terminal3BarText.Text = $"{t3.ToString("#.")}%";

                            barChartTop.Text = $"{vvvv[9]}";
                            barChartMid.Text = $"{vvvv[9] / 2}";

                            onDutyBar.Value = onDutyValue;
                            offDutyBar.Value = offDutyValue;
                            sickBar.Value = sickValue;
                            onLeaveBar.Value = onLeaveValue;

                            onDutyLabel.Content = vvvv[8];
                            offDutyLabel.Content = vvvv[9];
                            sickLabel.Content = vvvv[10];
                            onLeaveLabel.Content = vvvv[11];

                            morningStaffs.Content = $"{vvvv[12]} staff members";
                            afternoonStaffs.Content = $"{vvvv[13]} staff members";
                            nightStaffs.Content = $"{vvvv[14]} staff members";
                        });

                        await Task.Delay(1000);
                    }
                });
            }
            catch (Exception ex)
            {
                // Supress
            }
        }
    }
}
