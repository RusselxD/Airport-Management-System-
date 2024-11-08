using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
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
        private DoubleAnimation heightAnimation;
        private DoubleAnimation opacityAnimation;

        private bool departureIsShown;

        public FlightsControl(SqlConnection sqlConnection)
        {
            InitializeComponent();

            departureIsShown = true;

            this.heightAnimation = new DoubleAnimation
            {
                From = 0,
                To = 55, // Target height, adjust as needed
                Duration = TimeSpan.FromSeconds(0.4), // Duration of the animation
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } // Smooth easing
            };

            this.opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
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

        private Grid Create_New_Grid()
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.95, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.65, GridUnitType.Star) });

            return grid;
        }

        private TextBlock createTextBlocks(string t, int p1, int p2, int colNum)
        {
            TextBlock tb = new TextBlock
            {
                Text = t,
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(p1, p2, 0, 0)
            };
            Grid.SetColumn(tb, colNum);

            return tb;
        }

        private Border createNewRow()
        {
            Border border = new Border
            {
                Height = 55,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = Brushes.Black
            };

            Grid grid = Create_New_Grid();

            grid.Children.Add(createTextBlocks("AB 223", 20, 17, 0));
            grid.Children.Add(createTextBlocks("London (LHR)", 0, 17, 1));
            grid.Children.Add(createTextBlocks("10:30", 0, 17, 2));

            TextBlock status = new TextBlock
            {
                Text = "On Time",
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 20,
                Padding = new Thickness(0, 17, 0, 0)
            };
            Grid.SetColumn(status, 3);
            grid.Children.Add(status);

            grid.Children.Add(createTextBlocks("A3", 0, 17, 4));
            grid.Children.Add(createTextBlocks("2", 0, 17, 5));

            border.Child = grid;

            return border;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Border newRow = createNewRow();

            

            flightsStackPanel.Children.Add(newRow);

            newRow.BeginAnimation(Border.HeightProperty, heightAnimation);
            newRow.BeginAnimation(Border.OpacityProperty, opacityAnimation);



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
