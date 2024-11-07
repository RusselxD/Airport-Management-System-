using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    /// Interaction logic for FlightsControl.xaml
    /// </summary>
    public partial class FlightsControl : UserControl
    {
        public FlightsControl(SqlConnection sqlConnection)
        {
            InitializeComponent();
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(searchBox.Text == "Search Flight")
            {
                searchBox.Text = "";
                searchBox.Foreground = Brushes.Black;
            }
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.Text = "Search Flight";
                searchBox.Foreground = Brushes.Gray;
            }
        }

        private void Search_Icon_Click(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Border border = new Border
            {
                Height = 55,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = Brushes.Black
            };

            // Create Grid
            Grid grid = new Grid();

            // Define Grid Column Definitions
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.95, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.65, GridUnitType.Star) });

            // Define Row Definition (only one row in this case)
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Create TextBlocks for each column
            TextBlock flightNumber = new TextBlock
            {
                Text = "AB 223",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(20, 17, 0, 0)
            };
            Grid.SetColumn(flightNumber, 0);
            grid.Children.Add(flightNumber);

            TextBlock destination = new TextBlock
            {
                Text = "London (LHR)",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(0, 17, 0, 0)
            };
            Grid.SetColumn(destination, 1);
            grid.Children.Add(destination);

            TextBlock time = new TextBlock
            {
                Text = "10:30",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(0, 17, 0, 0)
            };
            Grid.SetColumn(time, 2);
            grid.Children.Add(time);

            TextBlock status = new TextBlock
            {
                Text = "On Time",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(0, 17, 0, 0)
            };
            Grid.SetColumn(status, 3);
            grid.Children.Add(status);

            TextBlock gate = new TextBlock
            {
                Text = "A3",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(0, 17, 0, 0)
            };
            Grid.SetColumn(gate, 4);
            grid.Children.Add(gate);

            TextBlock terminal = new TextBlock
            {
                Text = "2",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(0, 17, 0, 0)
            };
            Grid.SetColumn(terminal, 5);
            grid.Children.Add(terminal);

            // Add Grid to Border
            border.Child = grid;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = 0,
                To = 55, // Target height, adjust as needed
                Duration = TimeSpan.FromSeconds(0.4), // Duration of the animation
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } // Smooth easing
            };
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            flightsStackPanel.Children.Add(border);
            
            border.BeginAnimation(Border.HeightProperty, heightAnimation);
            border.BeginAnimation(Border.OpacityProperty, opacityAnimation);

            i++;
            if(i > 9)
            {
                flightsBorder.Height += 55;
                outerMostGrid.Height += 55;
            }
        }

        private int i = 1;

    }
}
