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
    /// Interaction logic for HomePageControl.xaml
    /// </summary>
    public partial class HomePageControl : UserControl
    {
        private SqlConnection sqlConnection;
        private bool appIsRunning = true;

        public HomePageControl(SqlConnection sqlConnection)
        {
            InitializeComponent();
            this.sqlConnection = sqlConnection;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                QueryAlertsTable();
            }).Start();
        }

        private int a = 0;

        void QueryAlertsTable()
        {
            int l1;

            while (appIsRunning)
            {
                 l1 = 0;

                string alertsQuery = "SELECT * FROM alerts_table";
                using (SqlCommand command1 = new SqlCommand(alertsQuery, sqlConnection))
                using (SqlDataReader alertsReader = command1.ExecuteReader())
                {
                    while (alertsReader.Read())
                    {
                        l1++;

                        if (l1 > a)
                        {
                            a = l1;
                            string alertMessage = alertsReader[0].ToString();
                            int alertCode = Convert.ToInt16(alertsReader[1]);

                            Dispatcher.InvokeAsync(() =>
                            {
                                AddAlert(alertMessage, alertCode);
                            });
                        }
                    }

                }

                Thread.Sleep(2000);
            }

        }

        private void AddAlert(string alertMessage, int alertDegree)
        {
            // Create the Border for the alert
            Border alertBorder = new Border
            {
                BorderThickness = new Thickness(1),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(40, 0, 40, 9),
            };

            // Create the Grid for the alert content
            Grid alertGrid = new Grid();

            // Add Image to the alert
            Image alertIcon = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Icons/alert-icon.png")),
                Width = 36,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(13, 9, 0, 10)
            };
            alertGrid.Children.Add(alertIcon);

            // Add Text to the alert
            TextBlock alertText = new TextBlock
            {
                Text = alertMessage,
                FontSize = 20,
                FontFamily = new FontFamily("Ubuntu"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(65, 0, 40, 0),
                Padding = new Thickness(0, 3, 0, 0)
            };

            alertText.Foreground = alertDegree == 1 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF71C1C")) :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE37E01"));

            alertGrid.Children.Add(alertText);

            // Add the Grid to the Border
            alertBorder.Child = alertGrid;

            // Add the alert Border to the StackPanel (AlertsPanel)
            AlertsPanel.Children.Add(alertBorder);

            x += 60;
            dashboardInnerGrid.RowDefinitions[1].Height = new GridLength(x);

        }

        private int x = 160;

        private void addRecentAct(string message)
        {
            Border border = new Border
            {
                BorderThickness = new Thickness(1),
                Height = 40,
                Margin = new Thickness(38, 0, 38, 5),
                VerticalAlignment = VerticalAlignment.Top,
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5, 5, 5, 5)
            };

            // Create the TextBlock element
            TextBlock textBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Text = message,
                Foreground = new SolidColorBrush(Color.FromRgb(109, 109, 109)),
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 16,
                Padding = new Thickness(20, 11, 0, 0)
            };

            // Add the TextBlock to the Border
            border.Child = textBlock;

            // Assuming you have a parent container (like a Grid or StackPanel) to add this Border to
            RecentActivitiesPanel.Children.Add(border);

            y += 45;
            dashboardInnerGrid.RowDefinitions[2].Height = new GridLength(y);

        }

        private int y = 280;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            addRecentAct("Gate B2 assigned to Flight BA 233.");
        }
    }
}
