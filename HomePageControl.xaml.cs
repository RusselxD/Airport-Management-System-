using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public HomePageControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddAlert("This is an alert message nigga.");
        }

        private void AddAlert(string alertMessage)
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
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF71C1C")),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(65, 0, 40, 0),
                Padding = new Thickness(0, 3, 0, 0)
            };
            alertGrid.Children.Add(alertText);

            // Add the Grid to the Border
            alertBorder.Child = alertGrid;

            // Add the alert Border to the StackPanel (AlertsPanel)
            AlertsPanel.Children.Add(alertBorder);

            a += 60;
            dashboardInnerGrid.RowDefinitions[1].Height = new GridLength(a);

        }

        private int a = 160;

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

            b += 45;
            dashboardInnerGrid.RowDefinitions[2].Height = new GridLength(b);

        }

        private int b = 280;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            addRecentAct("Gate B2 assigned to Flight BA 233.");
        }
    }
}
