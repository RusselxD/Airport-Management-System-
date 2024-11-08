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
using System.Windows.Media.Animation;
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
        private bool appIsRunning;

        private Border defaultAlertMessage;

        private int alerts = 0;
        private Dictionary<string, Border> alertsMap = new Dictionary<string, Border>();

        private int alertsRow = 165;
        private int actsRow = 280;

        SqlCommand alertsSelectQuery;
        // TODO: sqlcommand dispose properly


        // ------------------------------------- DEFAULT, REUSABLE ELEMENTS ------------------------------------- //

        Thickness actPadding = new Thickness(50, 3, 0, 0);
        Thickness actMargin = new Thickness(0, 0, 0, 5);
        SolidColorBrush actColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6D6D6D"));

        SolidColorBrush greenAlertBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFCFCF"));
        SolidColorBrush redAlertBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF91FFAF"));

        CornerRadius alertBorderRadius = new CornerRadius(5);
        Thickness alertMarginThickness = new Thickness(40, 0, 40, 9);

        FontFamily ubuntu = new FontFamily("Ubuntu");

        Thickness marginThickness = new Thickness(61, 0, 51, 0);
        Thickness paddingThickness = new Thickness(0, 3, 0, 0);

        SolidColorBrush alertTextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1EA843"));

        Thickness alertIconMargin = new Thickness(15, 0, 0, 0);
        BitmapImage alertIconImage = new BitmapImage(new Uri("pack://application:,,,/Icons/alert-icon.png"));
        BitmapImage alertDeleteIconImage = new BitmapImage(new Uri("pack://application:,,,/Icons/delete-icon.png"));
        Thickness alertDeleteIconMargin = new Thickness(0, 0, 25, 0);

        // ------------------------------------------------------------------------------------------------------ //

        public HomePageControl(SqlConnection sqlConnection)
        {
            InitializeComponent();
            this.sqlConnection = sqlConnection;

            appIsRunning = true;
            InitializeDefaultElements();

            defaultAlertMessage = GetDefaultAlertMessage();
            AlertsPanel.Children.Add(defaultAlertMessage);

            alertsSelectQuery = new SqlCommand("SELECT * FROM alerts_table", sqlConnection);

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                QueryAlertsTable();
            }).Start();

            RecentActivitiesPanel.Children.Add(GetRecentActivityText("Logged in as airport_admin"));
        }

        // hover effect for delete Icon
        private Style deleteIconStyle;

        private void InitializeDefaultElements()
        {
            // ------------------------- DEFAULT, REUSABLE ELEMENTS ------------------------- // 







            // ------------------------------------------------------------------------------ //
            // ------------------------------------------------------------------------------ //



            // ------------------------- DELETE ICON HOVER EFFECTS ------------------------- // 

            this.deleteIconStyle = new Style(typeof(Image));
            deleteIconStyle.Setters.Add(new Setter(Image.RenderTransformProperty, new ScaleTransform(1, 1)));
            deleteIconStyle.Setters.Add(new Setter(Image.RenderTransformOriginProperty, new Point(0.5, 0.5)));

            // Create Trigger for MouseOver event
            Trigger mouseOverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };

            // Define the animations for MouseOver
            DoubleAnimation scaleUpX = new DoubleAnimation { To = 1.05, Duration = TimeSpan.FromSeconds(0.10) };
            DoubleAnimation scaleUpY = new DoubleAnimation { To = 1.05, Duration = TimeSpan.FromSeconds(0.10) };
            Storyboard mouseOverStoryboard = new Storyboard();
            Storyboard.SetTargetProperty(scaleUpX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(scaleUpY, new PropertyPath("RenderTransform.ScaleY"));
            mouseOverStoryboard.Children.Add(scaleUpX);
            mouseOverStoryboard.Children.Add(scaleUpY);
            mouseOverTrigger.EnterActions.Add(new BeginStoryboard { Storyboard = mouseOverStoryboard });

            // Define animations for MouseLeave (exit actions)
            DoubleAnimation scaleDownX = new DoubleAnimation { To = 1, Duration = TimeSpan.FromSeconds(0.10) };
            DoubleAnimation scaleDownY = new DoubleAnimation { To = 1, Duration = TimeSpan.FromSeconds(0.10) };
            Storyboard mouseLeaveStoryboard = new Storyboard();
            Storyboard.SetTargetProperty(scaleDownX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(scaleDownY, new PropertyPath("RenderTransform.ScaleY"));
            mouseLeaveStoryboard.Children.Add(scaleDownX);
            mouseLeaveStoryboard.Children.Add(scaleDownY);
            mouseOverTrigger.ExitActions.Add(new BeginStoryboard { Storyboard = mouseLeaveStoryboard });

            // Add trigger to the style
            deleteIconStyle.Triggers.Add(mouseOverTrigger);

            // ---------------------------------------------------------------------------- //
            // ---------------------------------------------------------------------------- //

        }

        private Border GetDefaultAlertMessage()
        {
            Border border = Get_Alert_Border();

            Grid alertGrid = new Grid();
            TextBlock textBlock = Get_Alert_TextBlock("There are no alerts currently.");

            Rectangle greenCircle = new Rectangle
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 20,
                Width = 20,
                RadiusX = 50,
                RadiusY = 50,
                Margin = new Thickness(19, 0, 0, 0),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1EA843"))
            };

            alertGrid.Children.Add(textBlock);
            alertGrid.Children.Add(greenCircle);

            border.Child = alertGrid;
            return border;
        }

        private TextBlock GetRecentActivityText(string activityText)
        {
            TextBlock act = new TextBlock
            {
                Text = $"{DateTime.Now.ToString("HH:mm:ss")} - {activityText}",
                FontFamily = ubuntu,
                FontSize = 17,
                Foreground = actColor,
                Padding = actPadding,
                Margin = actMargin,
                Height = 27
            };
            return act;
        }

        void QueryAlertsTable()
        {
            int l1;

            while (appIsRunning)
            {
                l1 = 0;

                using (SqlDataReader alertsReader = alertsSelectQuery.ExecuteReader())
                {
                    while (alertsReader.Read())
                    {
                        l1++;

                        if (l1 > alerts)
                        {
                            alerts = l1;

                            if (alerts == 1)
                            {
                                Dispatcher.InvokeAsync(() =>
                                {
                                    AlertsPanel.Children.Clear();
                                    alertsContainer.Background = greenAlertBackground;
                                });
                            }
                            else
                            {
                                Dispatcher.InvokeAsync(() =>
                                {
                                    alertsContainer.Height += 59;

                                    alertsRow += 59;
                                    dashboardInnerGrid.RowDefinitions[1].Height = new GridLength(alertsRow);
                                });
                            }

                            int alertID = Convert.ToInt16(alertsReader[0]);
                            string alertMessage = alertsReader[1].ToString();
                            int alertCode = Convert.ToInt16(alertsReader[2]);

                            Dispatcher.InvokeAsync(() =>
                            {
                                AddAlert(alertID, alertMessage, alertCode);
                            });
                        }

                    }
                }
                Thread.Sleep(1000);
            }

        }

        private async void Delete_Alert(object sender, RoutedEventArgs e)
        {
            string name = (sender as Image).Name;

            // for SQL ID
            int alertID = Convert.ToInt16(name.Substring(1));

            // delete from SQL
            await Delete_Alert_Query(alertID);

            AlertsPanel.Children.Remove(alertsMap[name]);

            alerts--;
            if (alerts <= 0)
            {
                AlertsPanel.Children.Add(defaultAlertMessage);
                alertsContainer.Background = redAlertBackground;
            }
            else
            {
                AlertsPanel.Height -= 59;
                alertsContainer.Height -= 59;

                alertsRow -= 59;
                dashboardInnerGrid.RowDefinitions[1].Height = new GridLength(alertsRow);
            }
        }

        private async Task Delete_Alert_Query(int alert_ID)
        {
            await Task.Run(() =>
            {
                using (SqlCommand command = new SqlCommand($"DELETE FROM alerts_table WHERE alert_id = {alert_ID}", this.sqlConnection))
                {
                    command.ExecuteNonQuery();
                }
            });
        }

        private Border Get_Alert_Border()
        {
            Border alertBorder = new Border
            {
                Height = 50,
                Background = Brushes.White,
                CornerRadius = alertBorderRadius,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = alertMarginThickness
            };
            return alertBorder;
        }

        private TextBlock Get_Alert_TextBlock(string message)
        {
            TextBlock alertTextBlock = new TextBlock
            {
                Text = message,
                FontSize = 20,
                FontFamily = ubuntu,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = marginThickness,
                Padding = paddingThickness,
                Foreground = alertTextForeground
            };
            return alertTextBlock;
        }

        private Image Get_Alert_Icon()
        {
            Image alertIcon = new Image
            {
                Source = alertIconImage,
                Margin = alertIconMargin,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 23,
                Width = 26
            };
            return alertIcon;
        }

        private Image Get_Alert_Delete_Icon()
        {
            Image alertDeleteIcon = new Image
            {
                Source = alertDeleteIconImage,
                Margin = alertDeleteIconMargin,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 30,
                Width = 31,
                Cursor = Cursors.Hand
            };
            return alertDeleteIcon;
        }

        private void AddAlert(int alertID, string alertMessage, int alertDegree)
        {
            Border border = Get_Alert_Border();

            Grid alertGrid = new Grid();

            TextBlock alertText = Get_Alert_TextBlock(alertMessage);
            alertText.Foreground = alertDegree == 1 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF71C1C")) :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE37E01"));

            Image alertIcon = Get_Alert_Icon();
            Image deleteIcon = Get_Alert_Delete_Icon();

            deleteIcon.MouseDown += Delete_Alert;
            deleteIcon.Style = this.deleteIconStyle;

            alertGrid.Children.Add(alertText);
            alertGrid.Children.Add(alertIcon);
            alertGrid.Children.Add(deleteIcon);

            border.Child = alertGrid;

            AlertsPanel.Height += 59;
            AlertsPanel.Children.Add(border);

            // this is for accessing the border by name once deleted
            string name = $"_{alertID}";
            deleteIcon.Name = name;
            border.Name = name;

            alertsMap.Add(name, border);
        }

        private void addRecentAct(string message)
        {
            TextBlock newActivity = GetRecentActivityText(message);

            RecentActivitiesPanel.Children.Add(newActivity);
            RecentActivitiesPanel.Height += 32;

            actsRow += 32;
            dashboardInnerGrid.RowDefinitions[2].Height = new GridLength(actsRow);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            addRecentAct("Assigned Flight Nigga to Gate NIGGA");
        }
    }
}
