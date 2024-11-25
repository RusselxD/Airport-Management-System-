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
    /// Interaction logic for StaffControl.xaml
    /// </summary>
    public partial class StaffControl : UserControl
    {
        public StaffControl(SqlConnection sqlConnection)
        {
            InitializeComponent();
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Role_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBlock tb)
            {
                string role = tb.Text;
                header.Text = role;
            }
        }

        private bool rolesIsOpen = false;

        private void Role_DropDown_Click(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation showRoles = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300)) // Duration of 2 seconds
            };

            if(rolesIsOpen)
            {
                RoleDropDown.CornerRadius = new CornerRadius(8, 8, 8, 8);
                showRoles.From = 138;
                showRoles.To = 0;
                rolesIsOpen = false;
            }
            else
            {
                RoleDropDown.CornerRadius = new CornerRadius(8, 8, 0, 0);
                showRoles.From = 0;
                showRoles.To = 138;
                rolesIsOpen = true;
            }

            Roles.BeginAnimation(HeightProperty, showRoles);

        }

        private void Shift_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private bool shiftsIsOpen = false;

        private void Shift_DropDown_Click(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation showShifts = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300)) // Duration of 2 seconds
            };

            if (shiftsIsOpen)
            {
                ShiftsDropDown.CornerRadius = new CornerRadius(8, 8, 8, 8);
                showShifts.From = 104;
                showShifts.To = 0;
                shiftsIsOpen = false;
            }
            else
            {
                ShiftsDropDown.CornerRadius = new CornerRadius(8, 8, 0, 0);
                showShifts.From = 0;
                showShifts.To = 104;
                shiftsIsOpen = true;
            }

            Shifts.BeginAnimation(HeightProperty, showShifts);
        }

        private void Status_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private bool statusIsOpen = false;

        private void Status_DropDown_Click(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation showStatus = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300)) // Duration of 2 seconds
            };

            if (statusIsOpen)
            {
                StatusDropDown.CornerRadius = new CornerRadius(8, 8, 8, 8);
                showStatus.From = 138;
                showStatus.To = 0;
                statusIsOpen = false;
            }
            else
            {
                StatusDropDown.CornerRadius = new CornerRadius(8, 8, 0, 0);
                showStatus.From = 0;
                showStatus.To = 138;
                statusIsOpen = true;
            }

            Status.BeginAnimation(HeightProperty, showStatus);
        }
    }
}
