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
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
                Margin = new Thickness(0, 0, 20, 10),
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
                Margin = new Thickness(65, 0, 0, 0),
                Padding = new Thickness(0, 3, 0, 0)
            };
            alertGrid.Children.Add(alertText);

            // Add the Grid to the Border
            alertBorder.Child = alertGrid;

            // Add the alert Border to the StackPanel (AlertsPanel)
            AlertsPanel.Children.Add(alertBorder);

            dashboardInnerGrid.Height += 150;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddAlert("This is an alert message");
        }
    }
}
