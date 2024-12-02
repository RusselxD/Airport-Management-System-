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
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for FlightDetailsWindow.xaml
    /// </summary>
    public partial class FlightDetailsWindow : Window
    {
        private List<string> details;

        public FlightDetailsWindow(List<string> details)
        {
            this.details = details;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            PutLabels();
            Show();
        }

        private void PutLabels()
        {
            if (details[0].Equals("arrival"))
            {
                endPointLabel.Text = "Origin";
            }

            flight.Content = $"Flight {details[1]}";
            endPoint.Content = details[2];
            time.Content = details[3];
            status.Content = details[4];

            switch (details[4])
            {
                case "On Time":
                    status.Foreground = Brushes.Green;
                    break;
                case "Boarding":
                    status.Foreground = Brushes.Blue;
                    break;
                case "Delayed":
                    status.Foreground = Brushes.Red;
                    break;
                case "Landed":
                    status.Foreground = Brushes.Gray;
                    break;
            }

            gate.Content = details[5];
            terminal.Content = details[6];
            airline.Content = details[7];
        }
    }
}
