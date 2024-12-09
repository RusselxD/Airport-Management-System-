using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for EditStaffDetails.xaml
    /// </summary>
    public partial class EditStaffDetails : Window
    {
        Style hoverStyle;

        private readonly List<string> details;

        private readonly Dictionary<string, string> rolesMap = new Dictionary<string, string>()
        {
            {"Air Traffic Controller", "Flight Operations" },
            {"Aircraft Mechanic", "Maintenance" },
            {"Aircraft Technician", "Maintenance" },
            {"Airport Manager", "Management" },
            {"Baggage Handler", "Ground Operations" },
            {"Cabin Crew", "Cabin Crew" },
            {"Cargo Operations", "Cargo Operations" },
            {"Cargo Supervisor", "Cargo Operations" },
            {"Co-Pilot", "Flight Operations" },
            {"Customer Service", "Customer Service" },
            {"Dispatcher", "Flight Operations" },
            {"Fuel Technician", "Fueling" },
            {"Ground Crew", "Ground Operations" },
            {"Ground Manager", "Management" },
            {"Maintenance Engineer", "Maintenance" },
            {"Maintenance Manager", "Maintenance" },
            {"Passenger Assistance", "Ground Operations" },
            {"Passenger Service", "Customer Service" },
            {"Pilot", "Flight Operations" },
            {"Ramp Agent", "Ground Operations" },
            {"Security Officer", "Security" }
        };

        private readonly BitmapImage malePic = new BitmapImage(new Uri("/Icons/male-picture.png", UriKind.Relative));
        private readonly BitmapImage femalePic = new BitmapImage(new Uri("/Icons/female-picture.png", UriKind.Relative));

        private StaffControl staffControl;

        public EditStaffDetails(List<string> details, StaffControl staffControl)
        {
            this.staffControl = staffControl;
            this.details = details;
            InitializeComponent();
            InitializeUsableElements();
            AddRoles();
            AddStatus();
            AddShifts();
            InitializeDetails();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Show();
        }

        private void InitializeDetails()
        {
            name.Text = details[0];
            contactNo.Text = details[5];
            role.Content = details[1];
            shift.Content = details[2];
            status.Content = details[3];
            department.Text = details[4];
            if (details[6].Equals("Male"))
            {
                picture.Source = malePic;
                male.IsChecked = true;
            }
            else
            {
                picture.Source = femalePic;
                female.IsChecked = true;
            }
        }

        private DoubleAnimation showRoles;
        private bool rolesIsOpen = false;

        private DoubleAnimation showStatus;
        private bool statusIsOpen = false;

        private DoubleAnimation showShift;
        private bool shiftIsOpen = false;

        private void InitializeUsableElements()
        {
            showRoles = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };
            showRoles.Completed += (s, args) =>
            {
                if (!rolesIsOpen)
                {
                    roleBorder.CornerRadius = new CornerRadius(5);
                }
            };

            showStatus = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };
            showStatus.Completed += (s, args) =>
            {
                if (!statusIsOpen)
                {
                    statusBorder.CornerRadius = new CornerRadius(5);
                }
            };

            showShift = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };
            showShift.Completed += (s, args) =>
            {
                if (!shiftIsOpen)
                {
                    shiftBorder.CornerRadius = new CornerRadius(5);
                }
            };

            hoverStyle = new Style(typeof(Label));
            hoverStyle.Setters.Add(new Setter(Label.BackgroundProperty, Brushes.White));
            Trigger isMouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            isMouseOverTrigger.Setters.Add(new Setter(Label.BackgroundProperty, new SolidColorBrush(Color.FromRgb(235, 235, 235))));
            hoverStyle.Triggers.Add(isMouseOverTrigger);
        }

        private void AddRoles()
        {
            string[] rolesList = new string[]
            {
                "Air Traffic Controller",
                "Aircraft Mechanic",
                "Aircraft Technician",
                "Airport Manager",
                "Baggage Handler",
                "Cabin Crew",
                "Cargo Operations",
                "Cargo Supervisor",
                "Co-Pilot",
                "Customer Service",
                "Dispatcher",
                "Fuel Technician",
                "Ground Crew",
                "Ground Manager",
                "Maintenance Engineer",
                "Maintenance Manager",
                "Passenger Assistance",
                "Passenger Service",
                "Pilot",
                "Ramp Agent",
                "Security Officer",
            };

            foreach (string r in rolesList)
            {
                Label role = new Label
                {
                    Cursor = Cursors.Hand,
                    Content = r,
                    FontFamily = new FontFamily("Ubuntu"),
                    FontSize = 13,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 180,
                    Height = 32,
                    Padding = new Thickness(5, 6, 5, 5),
                    Style = hoverStyle
                };

                role.MouseDown += (sender, e) =>
                {
                    ChooseRole(sender, e);
                };

                roles.Children.Add(role);
            }

        }

        private void AddStatus()
        {
            string[] statusList = new string[]
            {
                "On Duty",
                "Off Duty",
                "Sick",
                "On Leave"
            };

            foreach (string r in statusList)
            {
                Label status = new Label
                {
                    Cursor = Cursors.Hand,
                    Content = r,
                    FontFamily = new FontFamily("Ubuntu"),
                    FontSize = 13,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 180,
                    Height = 32,
                    Padding = new Thickness(5, 6, 5, 5),
                    Style = hoverStyle
                };

                status.MouseDown += (sender, e) =>
                {
                    ChooseStatus(sender, e);
                };

                statuses.Children.Add(status);
            }
        }

        private void AddShifts()
        {
            string[] shiftlist = new string[]
            {
                "Morning",
                "Afternoon",
                "Night"
            };

            foreach (string r in shiftlist)
            {
                Label shift = new Label
                {
                    Cursor = Cursors.Hand,
                    Content = r,
                    FontFamily = new FontFamily("Ubuntu"),
                    FontSize = 13,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 180,
                    Height = 32,
                    Padding = new Thickness(5, 6, 5, 5),
                    Style = hoverStyle
                };

                shift.MouseDown += (sender, e) =>
                {
                    ChooseShift(sender, e);
                };

                shifts.Children.Add(shift);
            }
        }

        private void ChooseRole(object sender, MouseButtonEventArgs e)
        {
            string roleChosen = ((Label)sender).Content.ToString();
            role.Content = roleChosen;
            department.Text = rolesMap[roleChosen];

            showRoles.From = 96;
            showRoles.To = 0;

            rolesIsOpen = false;
            rolesDropDown.BeginAnimation(HeightProperty, showRoles);
        }

        private void ChooseStatus(object sender, MouseButtonEventArgs e)
        {
            string statusChosen = ((Label)sender).Content.ToString();
            status.Content = statusChosen;

            showStatus.From = 96;
            showStatus.To = 0;

            statusIsOpen = false;
            statusDropDown.BeginAnimation(HeightProperty, showStatus);
        }

        private void ChooseShift(object sender, MouseButtonEventArgs e)
        {
            shift.Content = ((Label)sender).Content.ToString();

            showShift.From = 96;
            showShift.To = 0;

            shiftIsOpen = false;
            shiftDropDown.BeginAnimation(HeightProperty, showShift);
        }

        private void OpenRoles(object sender, MouseButtonEventArgs e)
        {
            if (rolesIsOpen)
            {
                showRoles.From = 96;
                showRoles.To = 0;
            }
            else
            {
                roleBorder.CornerRadius = new CornerRadius(5, 5, 0, 0);
                showRoles.From = 0;
                showRoles.To = 96;
            }

            rolesIsOpen = !rolesIsOpen;
            rolesDropDown.BeginAnimation(HeightProperty, showRoles);
        }

        private void Open_Status(object sender, MouseButtonEventArgs e)
        {
            if (statusIsOpen)
            {
                showStatus.From = 96;
                showStatus.To = 0;
            }
            else
            {
                statusBorder.CornerRadius = new CornerRadius(5, 5, 0, 0);
                showStatus.From = 0;
                showStatus.To = 96;
            }

            statusIsOpen = !statusIsOpen;
            statusDropDown.BeginAnimation(HeightProperty, showStatus);
        }

        private void Open_Shifts(object sender, MouseButtonEventArgs e)
        {
            if (shiftIsOpen)
            {
                showShift.From = 96;
                showShift.To = 0;
            }
            else
            {
                shiftBorder.CornerRadius = new CornerRadius(5, 5, 0, 0);
                showShift.From = 0;
                showShift.To = 96;
            }

            shiftIsOpen = !shiftIsOpen;
            shiftDropDown.BeginAnimation(HeightProperty, showShift);
        }

        private void male_Checked(object sender, RoutedEventArgs e)
        {
            picture.Source = malePic;
        }

        private void female_Checked(object sender, RoutedEventArgs e)
        {
            picture.Source = femalePic;
        }

        private async void Confirm_Update(object sender, MouseButtonEventArgs e)
        {
            if (!Valid_Input())
            {
                return;
            }

            try
            {
                string gender = male.IsChecked == true ? "fas" : "fasf";

                using (SqlCommand command = new SqlCommand("", MainWindow.sqlConnection))
                {
                    command.CommandText = "UPDATE staffs_table SET name_col = @name, role_col = @role, shift_col = @shift, status_col = @status, department_col = @department, contact_no = @contact, gender = @gender WHERE name_col = @origName";
                    command.Parameters.AddWithValue("@origName", details[0]);
                    command.Parameters.AddWithValue("@name", name.Text);
                    command.Parameters.AddWithValue("@role", role.Content);
                    command.Parameters.AddWithValue("@shift", shift.Content);
                    command.Parameters.AddWithValue("@status", status.Content);
                    command.Parameters.AddWithValue("@department", department.Text);
                    command.Parameters.AddWithValue("@contact", contactNo.Text);
                    command.Parameters.AddWithValue("@gender", gender);

                    int rows = await command.ExecuteNonQueryAsync();

                    if (rows == 1)
                    {
                        staffControl.UpdateHomePage($"Updated {details[0]}'s details.");
                        MessageBox.Show("Staff details updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update staff details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Valid_Input()
        {
            if (name.Text.Length == 0)
            {
                MessageBox.Show("Name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (contactNo.Text.Length == 0)
            {
                MessageBox.Show("Contact number cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}