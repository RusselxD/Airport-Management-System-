using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
    /// Interaction logic for GatesControl.xaml
    /// </summary>
    public partial class GatesControl : UserControl
    {
        private Dictionary<int, Border> gatesMap;
        private Dictionary<int, TextBlock> statusMap;
        private Dictionary<int, TextBlock> messageMap;

        private SolidColorBrush green;
        private SolidColorBrush red;
        private SolidColorBrush orange;

        private SolidColorBrush darkGreen;
        private SolidColorBrush darkRed;
        private SolidColorBrush darkOrange;

        private SqlCommand availableGatesQuery;
        private SqlCommand maintenanceGatesQuery;
        private SqlCommand occupiedGatesQuery;

        public GatesControl()
        {
            InitializeComponent();

            AssignGlobalValues();

            Populate_Maps();

            Task.Run(() => QueryGatesTable(MainWindow.cts.Token));
            Task.Run(() => Update_Available_Gates_Count(MainWindow.cts.Token));
            Task.Run(() => Update_Occupied_Gates_Count(MainWindow.cts.Token));
            Task.Run(() => Update_Maintenance_Gates_Count(MainWindow.cts.Token));
        }

        private void AssignGlobalValues()
        {
            gatesMap = new Dictionary<int, Border>();
            statusMap = new Dictionary<int, TextBlock>();
            messageMap = new Dictionary<int, TextBlock>();

            green = new SolidColorBrush(Color.FromRgb(186, 255, 190));
            red = new SolidColorBrush(Color.FromRgb(255, 192, 192));
            orange = new SolidColorBrush(Color.FromRgb(255, 215, 166));

            darkGreen = new SolidColorBrush(Color.FromRgb(30, 168, 67));
            darkRed = new SolidColorBrush(Color.FromRgb(247, 28, 28));
            darkOrange = new SolidColorBrush(Color.FromRgb(248, 142, 12));

            availableGatesQuery = new SqlCommand("SELECT COUNT(1) FROM gates_table WHERE status_col = 1;", MainWindow.sqlConnection);
            maintenanceGatesQuery = new SqlCommand("SELECT COUNT(1) FROM gates_table WHERE status_col = 2;", MainWindow.sqlConnection);
            occupiedGatesQuery = new SqlCommand("SELECT COUNT(1) FROM gates_table WHERE status_col = 3;", MainWindow.sqlConnection);
        }

        private void Populate_Maps()
        {
            int k = 1;
            char c = 'a';
            for (int i = 1; i <= 18; i++)
            {
                string borderName = $"t{k}{c}";
                Border b = FindName(borderName) as Border;
                gatesMap.Add(i, b);

                string statusName = $"{borderName}Status";
                TextBlock status = FindName(statusName) as TextBlock;
                statusMap.Add(i, status);

                string messageName = $"{borderName}Message";
                TextBlock message = FindName(messageName) as TextBlock;
                messageMap.Add(i, message);

                if (i == 6 || i == 12)
                {
                    k++;
                    c = 'a';
                }
                else
                {
                    c = (char)(c + 1);
                }
            }
        }

        private async Task QueryGatesTable(CancellationToken token)
        {
            using (SqlCommand gatesQuery = new SqlCommand("SELECT * FROM gates_table", MainWindow.sqlConnection))
            {
                using (SqlDataReader gatesReader = await gatesQuery.ExecuteReaderAsync(token))
                {
                    while (gatesReader.Read())
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            UpdateGate(Convert.ToInt16(gatesReader[0]), Convert.ToInt16(gatesReader[2]), gatesReader[3].ToString());
                        }).Task.Wait();
                    }
                }
            }
        }

        private async Task Update_Available_Gates_Count(CancellationToken token)
        {
            using (availableGatesQuery)
            {
                using (SqlDataReader availableCountReader = await availableGatesQuery.ExecuteReaderAsync(token))
                {
                    while (availableCountReader.Read())
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            availableCount.Text = availableCountReader[0].ToString();
                        }).Task.Wait();
                    }
                }
            }
        }

        private async Task Update_Maintenance_Gates_Count(CancellationToken token)
        {
            using (maintenanceGatesQuery)
            {
                using (SqlDataReader maintenanceCountReader = await maintenanceGatesQuery.ExecuteReaderAsync(token))
                {
                    while (maintenanceCountReader.Read())
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            maintenanceCount.Text = maintenanceCountReader[0].ToString();
                        }).Task.Wait();
                    }
                }
            }
        }

        private async Task Update_Occupied_Gates_Count(CancellationToken token)
        {
            using (occupiedGatesQuery)
            {
                using (SqlDataReader OccupiedCountReader = await occupiedGatesQuery.ExecuteReaderAsync(token))
                {
                    while (OccupiedCountReader.Read())
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            occupiedCount.Text = OccupiedCountReader[0].ToString();
                        }).Task.Wait();
                    }
                }
            }
        }

        private void UpdateGate(int index, int status, string details)
        {
            switch (status)
            {
                case 1:
                    gatesMap[index].Background = green;
                    statusMap[index].Text = "Available";
                    statusMap[index].Foreground = darkGreen;
                    break;
                case 2:
                    gatesMap[index].Background = orange;
                    statusMap[index].Text = "Maintenance";
                    statusMap[index].Foreground = darkOrange;
                    break;
                case 3:
                    gatesMap[index].Background = red;
                    statusMap[index].Text = "Occupied";
                    statusMap[index].Foreground = darkRed;
                    break;
            }
            messageMap[index].Text = details;
        }
    }
}
