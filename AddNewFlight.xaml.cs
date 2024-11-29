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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for AddNewFlight.xaml
    /// </summary>
    public partial class AddNewFlight : Window
    {
        DoubleAnimation showStatusChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        DoubleAnimation showGateChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        DoubleAnimation showTerminalChoices = new DoubleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };

        public AddNewFlight()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Show();

            

        }

        private void Choose_Status(object sender, MouseButtonEventArgs e)
        {
            status.Text = (sender as TextBlock).Text;
        }

        private bool statusChoicesIsOpen = false;

        private void Open_Status_Choices(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (statusChoicesIsOpen)
            {
                showStatusChoices.From = 105;
                showStatusChoices.To = 0;
            } 
            else
            {
                showStatusChoices.From = 0;
                showStatusChoices.To = 105;
                b.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }

            statusChoicesIsOpen = !statusChoicesIsOpen;

            showStatusChoices.Completed += (s, args) =>
            {
                if (!statusChoicesIsOpen)
                {
                    b.CornerRadius = new CornerRadius(8);
                }
            };

            statusChoices.BeginAnimation(Border.HeightProperty, showStatusChoices);        
        }

        private void Choose_Gate(object sender, MouseButtonEventArgs e)
        {
            gate.Text = (sender as TextBlock).Text;
        }

        private bool gateChoicesIsOpen = false;

        private void Open_Gate_Choices(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (gateChoicesIsOpen)
            {
                showGateChoices.From = 105;
                showGateChoices.To = 0;
            }
            else
            {
                showGateChoices.From = 0;
                showGateChoices.To = 105;
                b.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }

            gateChoicesIsOpen = !gateChoicesIsOpen;

            showGateChoices.Completed += (s, args) =>
            {
                if (!gateChoicesIsOpen)
                {
                    b.CornerRadius = new CornerRadius(8);
                }
            };

            gateChoices.BeginAnimation(Border.HeightProperty, showGateChoices);
        }

        private void Choose_Terminal(object sender, MouseButtonEventArgs e)
        {
            terminal.Text = (sender as TextBlock).Text;

            showTerminalChoices.From = 105;
            showTerminalChoices.To = 0;

            terminalChoicesIsOpen = !terminalChoicesIsOpen;

            terminalChoices.BeginAnimation(Border.HeightProperty, showTerminalChoices);
        }

        private bool terminalChoicesIsOpen = false;

        private void Open_Terminal_Choices(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (terminalChoicesIsOpen)
            {
                showTerminalChoices.From = 105;
                showTerminalChoices.To = 0;
            }
            else
            {
                showTerminalChoices.From = 0;
                showTerminalChoices.To = 105;
                b.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }

            terminalChoicesIsOpen = !terminalChoicesIsOpen;

            showTerminalChoices.Completed += (s, args) =>
            {
                if (!terminalChoicesIsOpen)
                {
                    b.CornerRadius = new CornerRadius(8);
                }
            };


            terminalChoices.BeginAnimation(Border.HeightProperty, showTerminalChoices);
        }
    }
}
