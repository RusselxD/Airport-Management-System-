using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for HomePageControl.xaml
    /// </summary>
    public partial class HomePageControl : UserControl
    {
        public bool appIsRunning { get; set; }

        private readonly Border defaultAlertMessage;

        private int alerts = 0;
        private readonly Dictionary<string, Border> alertsMap = new Dictionary<string, Border>();

        private int alertsRow = 165;
        private int actsRow = 280;

        // hover effect for delete Icon
        private Style deleteIconStyle;

        private Image deleteAllIcon;

        private DoubleAnimation alertHeightAnimation;
        private DoubleAnimation alertBorderOpacityAnimation;
        private DoubleAnimation alertOpacityAnimation;


        // ------------------------------------- DEFAULT, REUSABLE ELEMENTS ------------------------------------- //

        private readonly Thickness actPadding = new Thickness(50, 3, 0, 0);
        private readonly Thickness actMargin = new Thickness(0, 0, 0, 5);
        private readonly SolidColorBrush actColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6D6D6D"));

        private readonly SolidColorBrush greenAlertBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFCFCF"));
        private readonly SolidColorBrush redAlertBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF91FFAF"));

        private readonly CornerRadius alertBorderRadius = new CornerRadius(5);
        private readonly Thickness alertMarginThickness = new Thickness(40, 0, 40, 9);

        private readonly FontFamily ubuntu = new FontFamily("Ubuntu");

        private readonly Thickness marginThickness = new Thickness(61, 0, 51, 0);
        private readonly Thickness paddingThickness = new Thickness(0, 3, 0, 0);

        private readonly SolidColorBrush alertTextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1EA843"));

        private readonly Thickness alertIconMargin = new Thickness(15, 0, 0, 0);
        private readonly BitmapImage alertIconImage = new BitmapImage(new Uri("pack://application:,,,/Icons/alert-icon.png"));
        private readonly BitmapImage alertDeleteIconImage = new BitmapImage(new Uri("pack://application:,,,/Icons/delete-icon.png"));
        private readonly Thickness alertDeleteIconMargin = new Thickness(0, 0, 25, 0);

        // ------------------------------------------------------------------------------------------------------ //

        private readonly MainWindow window;

        public HomePageControl(MainWindow window)
        {
            this.window = window;

            InitializeComponent();

            appIsRunning = true;
            InitializeDefaultElements();

            defaultAlertMessage = GetDefaultAlertMessage();
            AlertsPanel.Children.Add(defaultAlertMessage);

            Task.Run(() => QueryAlertsTable(MainWindow.cts.Token));
            Task.Run(() => RefreshStats(MainWindow.cts.Token));

            RecentActivitiesPanel.Children.Add(GetRecentActivityText("Logged in as airport_admin"));
        }

        public async void RefreshStats(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int[] totals = new int[3];
                    string[] queries = new string[3]
                    {
                            "SELECT (SELECT COUNT(1) FROM departure_flights) + (SELECT COUNT(1) FROM arrival_flights WHERE NOT flight_status = 'Landed');",
                            "SELECT COUNT(1) FROM gates_table WHERE status_col = 3;",
                            "SELECT COUNT(1) FROM staffs_table WHERE status_col = 'On Duty';"
                    };

                    for (int i = 0; i < 3; i++)
                    {
                        using (SqlCommand activeFlightsQuery = new SqlCommand(queries[i], MainWindow.sqlConnection))
                        using (SqlDataReader activeFlightsReader = await activeFlightsQuery.ExecuteReaderAsync(token))
                        {
                            while (await activeFlightsReader.ReadAsync(token))
                            {
                                totals[i] = Convert.ToInt16(activeFlightsReader[0]);
                            }
                        }
                    }

                    await Dispatcher.InvokeAsync(() =>
                    {
                        activeFlightsCount.Text = totals[0].ToString();
                        occupiedGatesCount.Text = $"{totals[1]}";
                        staffOnDutyCount.Text = totals[2].ToString();
                    });
                });
            }
            catch (Exception)
            {
                // Supress
            }
        }

        private void InitializeDefaultElements()
        {
            // ------------------------- DELETE ICON HOVER EFFECTS ------------------------- // 

            this.deleteIconStyle = new Style(typeof(Image));

            // Set the default RenderTransform (ScaleTransform) for scaling
            this.deleteIconStyle.Setters.Add(new Setter(UIElement.RenderTransformProperty, new ScaleTransform(1, 1)));
            this.deleteIconStyle.Setters.Add(new Setter(UIElement.RenderTransformOriginProperty, new Point(0.5, 0.5)));

            // Define the Trigger for MouseOver (IsMouseOver)
            var trigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };

            // Create the MouseOver (scale up) storyboard
            var enterStoryboard = new Storyboard();
            var scaleUpX = new DoubleAnimation
            {
                To = 1.1,
                Duration = TimeSpan.FromSeconds(0.1)
            };
            Storyboard.SetTargetProperty(scaleUpX, new PropertyPath("RenderTransform.ScaleX"));
            enterStoryboard.Children.Add(scaleUpX);

            var scaleUpY = new DoubleAnimation
            {
                To = 1.1,
                Duration = TimeSpan.FromSeconds(0.1)
            };
            Storyboard.SetTargetProperty(scaleUpY, new PropertyPath("RenderTransform.ScaleY"));
            enterStoryboard.Children.Add(scaleUpY);

            // Create the MouseLeave (scale down) storyboard
            var exitStoryboard = new Storyboard();
            var scaleDownX = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.1)
            };
            Storyboard.SetTargetProperty(scaleDownX, new PropertyPath("RenderTransform.ScaleX"));
            exitStoryboard.Children.Add(scaleDownX);

            var scaleDownY = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.1)
            };
            Storyboard.SetTargetProperty(scaleDownY, new PropertyPath("RenderTransform.ScaleY"));
            exitStoryboard.Children.Add(scaleDownY);

            // Assign animations to Enter and Exit actions of the trigger
            trigger.EnterActions.Add(new BeginStoryboard { Storyboard = enterStoryboard });
            trigger.ExitActions.Add(new BeginStoryboard { Storyboard = exitStoryboard });

            // Add the trigger to the style
            this.deleteIconStyle.Triggers.Add(trigger);

            // ---------------------------------------------------------------------------- //

            // --------------------------------  ANIMATIONS  -------------------------------- //

            this.alertHeightAnimation = new DoubleAnimation
            {
                From = 0,
                To = 50, // Target height, adjust as needed
                Duration = TimeSpan.FromSeconds(0.3), // Duration of the animation
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } // Smooth easing
            };

            this.alertBorderOpacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            this.alertOpacityAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // ------------------------------------------------------------------------------ //

            this.deleteAllIcon = new Image
            {
                Source = alertDeleteIconImage,
                Margin = new Thickness(0, 23, 48, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 29,
                Cursor = Cursors.Hand,
                Style = this.deleteIconStyle
            };
            this.deleteAllIcon.MouseDown += Delete_All_Alert;
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
                Text = $"{DateTime.Now:HH:mm:ss} - {activityText}",
                FontFamily = ubuntu,
                FontSize = 17,
                Foreground = actColor,
                Padding = actPadding,
                Margin = actMargin,
                Height = 27
            };
            return act;
        }

        private async Task QueryAlertsTable(CancellationToken token)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int counter;
                    using (SqlCommand alertsSelectQuery = new SqlCommand("SELECT * FROM alerts_table", MainWindow.sqlConnection))
                    {
                        while (!token.IsCancellationRequested)
                        {
                            try
                            {
                                using (SqlDataReader alertsReader = await alertsSelectQuery.ExecuteReaderAsync(token))
                                {
                                    counter = 0;

                                    while (await alertsReader.ReadAsync(token))
                                    {
                                        counter++;

                                        if (counter > alerts)
                                        {
                                            alerts = counter;

                                            if (alerts == 1)
                                            {
                                                await Dispatcher.InvokeAsync(() =>
                                                {
                                                    AlertsPanel.Children.Clear();
                                                    alertsGrid.Children.Add(deleteAllIcon);
                                                    alertsContainer.Background = greenAlertBackground;
                                                    alertsContainer.BeginAnimation(Border.OpacityProperty, alertBorderOpacityAnimation);
                                                });
                                            }
                                            else
                                            {
                                                await Dispatcher.InvokeAsync(() =>
                                                {
                                                    alertsContainer.Height += 59;

                                                    alertsRow += 59;
                                                    dashboardInnerGrid.RowDefinitions[1].Height = new GridLength(alertsRow);
                                                });
                                            }

                                            int alertID = Convert.ToInt16(alertsReader[0]);
                                            string alertMessage = alertsReader[1].ToString();
                                            int alertCode = Convert.ToInt16(alertsReader[2]);

                                            await Dispatcher.InvokeAsync(() =>
                                            {
                                                AddAlert(alertID, alertMessage, alertCode);
                                            });
                                        }
                                    }
                                }
                            }
                            catch (TaskCanceledException)
                            {
                                break;
                            }

                            if (token.IsCancellationRequested)
                                break;

                            await Task.Delay(10, token);
                        }
                    }

                });
            }
            catch (Exception)
            {
                // Supress
            }
        }

        private void Delete_All_Alert(object sender, MouseButtonEventArgs e)
        {
            List<string> keys = alertsMap.Keys.ToList();

            foreach (string key in keys)
            {
                Delete_Alert(key);
            }
        }

        private async void Delete_Alert(string name)
        {
            Border borderDelete = alertsMap[name];

            alertsMap.Remove(name);

            // for SQL ID
            int alertID = Convert.ToInt16(name.Substring(1));

            // delete from SQL
            await Delete_Alert_Query(alertID);

            var storyboard = new Storyboard();
            storyboard.Children.Add(alertOpacityAnimation);
            Storyboard.SetTarget(alertOpacityAnimation, borderDelete);
            Storyboard.SetTargetProperty(alertOpacityAnimation, new PropertyPath(Border.OpacityProperty));

            storyboard.Completed += (s, a) =>
            {
                AlertsPanel.Children.Remove(borderDelete);

                alerts--;
                if (alerts <= 0)
                {
                    AlertsPanel.Children.Add(defaultAlertMessage);
                    alertsGrid.Children.Remove(deleteAllIcon);
                    defaultAlertMessage.BeginAnimation(Border.HeightProperty, alertHeightAnimation);
                    alertsContainer.Background = redAlertBackground;
                    alertsContainer.BeginAnimation(Border.OpacityProperty, alertBorderOpacityAnimation);
                }
                else
                {
                    AlertsPanel.Height -= 59;
                    alertsContainer.Height -= 59;

                    alertsRow -= 59;
                    dashboardInnerGrid.RowDefinitions[1].Height = new GridLength(alertsRow);
                }

            };

            storyboard.Begin();
        }

        private void Delete_Single_Alert(object sender, RoutedEventArgs e)
        {
            (sender as Image).MouseDown -= Delete_Single_Alert;
            string name = (sender as Image).Name;
            Delete_Alert(name);
        }

        private async Task Delete_Alert_Query(int alert_ID)
        {
            await Task.Run(() =>
            {
                using (SqlCommand deleteAlertStatement = new SqlCommand($"DELETE FROM alerts_table WHERE alert_id = {alert_ID}", MainWindow.sqlConnection))
                {
                    deleteAlertStatement.ExecuteNonQuery();
                    deleteAlertStatement.Dispose();
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

            alertDeleteIcon.MouseDown += Delete_Single_Alert;
            alertDeleteIcon.Style = this.deleteIconStyle;
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

            alertGrid.Children.Add(alertText);
            alertGrid.Children.Add(alertIcon);
            alertGrid.Children.Add(deleteIcon);

            border.Child = alertGrid;

            AlertsPanel.Height += 59;
            AlertsPanel.Children.Add(border);

            border.BeginAnimation(Border.HeightProperty, alertHeightAnimation);

            // this is for accessing the border by name once deleted
            string name = $"_{alertID}";
            deleteIcon.Name = name;
            border.Name = name;

            alertsMap.Add(name, border);
        }

        public void addRecentAct(string message)
        {
            TextBlock newActivity = GetRecentActivityText(message);

            RecentActivitiesPanel.Children.Add(newActivity);
            RecentActivitiesPanel.Height += 32;

            actsRow += 32;
            dashboardInnerGrid.RowDefinitions[2].Height = new GridLength(actsRow);
        }

        private void Open_Add_Flight_Window(object sender, MouseButtonEventArgs e)
        {
            new AddNewFlight(this);
        }

        private void Go_To_Flights_Window(object sender, MouseButtonEventArgs e)
        {
            window.changeCurrentControl(1);
        }

        private void Go_To_Gates_Window(object sender, MouseButtonEventArgs e)
        {
            window.changeCurrentControl(3);
        }

        private void Go_To_Staffs_Window(object sender, MouseButtonEventArgs e)
        {
            window.changeCurrentControl(4);
        }

        private void Open_Assign_Gate_Table(object sender, MouseButtonEventArgs e)
        {
            new AssignGate(this);
        }

        public void Refresh_Gate_And_Flight_Controls()
        {
            window.Refresh_Flight_Control();
            window.Refresh_Gate_Control();
        }

        public void Refresh_Flight_Control()
        {
            window.Refresh_Flight_Control();
        }
    }
}
