using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for StaffDetailsWindow.xaml
    /// </summary>
    public partial class StaffDetailsWindow : Window
    {
        private List<string> details;
        public StaffDetailsWindow(List<string> details)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Title = details[0];
            this.details = details;
            InitializeComponent();
            Show();

            name.Content = details[0];
            role.Content = details[1];
            shift.Content = details[2];
            status.Content = details[3];
            switch (details[3])
            {
                case "On Duty":
                    status.Background = new SolidColorBrush(Color.FromRgb(186, 255, 167)); 
                    break;
                case "Off Duty":    
                    status.Background = new SolidColorBrush(Color.FromRgb(191, 191, 191));
                    break;
                default:
                    status.Background = new SolidColorBrush(Color.FromRgb(255, 192, 192));
                    break;
            }

            department.Content = details[4];
            contactNumber.Content = details[5];

            if (details[6].Equals("Female"))
            {
                picture.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/female-picture.png"));
            }
            else
            {
                picture.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/male-picture.png"));
            }
        }
    }
}
