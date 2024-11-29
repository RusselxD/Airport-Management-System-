using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for StaffControl.xaml
    /// </summary>
    public partial class StaffControl : UserControl
    {
        private List<List<string>> staffs;
        private List<List<string>> filteredStaffs;
        private bool filterIsOn;

        private bool roleFilterIsOn;
        private bool shiftFilterIsOn;
        private bool statusFilterIsOn;

        // -------------------------------------- STAFF BORDER REUSABLE COMPONENTS -------------------------------------- //
        private Thickness borderThickness = new Thickness(0, 0, 0, 1);
        private Brush black = Brushes.Black;

        private Thickness pad1 = new Thickness(20, 17, 0, 0);
        private Thickness pad2 = new Thickness(0, 17, 0, 0);
        private Thickness pad3 = new Thickness(10, 12, 0, 0);

        private Thickness statusBorderMargin = new Thickness(0, 5, 5, 5);
        private CornerRadius statusBorderRadius = new CornerRadius(5);

        private SolidColorBrush green = new SolidColorBrush(Color.FromRgb(186, 255, 167));
        private SolidColorBrush gray = new SolidColorBrush(Color.FromRgb(191, 191, 191));
        private SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 192, 192));
        // --------------------------------------------------------------------------------------------------------------- //

        public StaffControl(SqlConnection sqlConnection)
        {
            staffs = new List<List<string>>();
            filteredStaffs = new List<List<string>>();

            roleFilterIsOn = false;
            shiftFilterIsOn = false;
            statusFilterIsOn = false;

            filterIsOn = roleFilterIsOn || shiftFilterIsOn || statusFilterIsOn;
            InitializeComponent();

            Task.Run(() => QueryStaffsTable(MainWindow.cts.Token));

            InitializeAnimations();
        }

        private void InitializeAnimations()
        {
            showRoles = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };
            showRoles.Completed += (s, args) =>
            {
                if (!rolesIsOpen)
                {
                    RoleDropDown.CornerRadius = new CornerRadius(8);
                }
            };

            showShifts = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };
            showShifts.Completed += (s, args) =>
            {
                if (!shiftsIsOpen)
                {
                    ShiftsDropDown.CornerRadius = new CornerRadius(8);
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
                    StatusDropDown.CornerRadius = new CornerRadius(8);
                }
            };
        }

        DoubleAnimation showRoles;
        DoubleAnimation showShifts;
        DoubleAnimation showStatus;

        private void Get_Column_Definition(Grid g)
        {
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.15, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) });
        }

        FontFamily ubuntu = new FontFamily("Ubuntu");

        private TextBlock Make_TextBlock(string text, Thickness pad, int col)
        {
            TextBlock tb = new TextBlock()
            {
                Text = text,
                FontSize = 19,
                FontFamily = ubuntu,
                Padding = pad
            };
            Grid.SetColumn(tb, col);

            return tb;
        }

        private void Display_All_Staffs()
        {
            staffsPanel.Children.Clear();
            int staffsDisplayed = 0;

            foreach (List<string> staff in staffs)
            {
                if (staffsDisplayed >= 9)
                {
                    staffsBorder.Height += 55;
                    outerMostGrid.Height += 55;
                }

                Border b = new Border()
                {
                    Height = 55,
                    BorderThickness = borderThickness,
                    BorderBrush = black
                };

                Grid g = new Grid();
                Get_Column_Definition(g);

                TextBlock name = Make_TextBlock(staff[0], pad1, 0);
                TextBlock role = Make_TextBlock(staff[1], pad2, 1);
                TextBlock shift = Make_TextBlock(staff[2], pad2, 2);

                Border status = new Border()
                {
                    Margin = statusBorderMargin,
                    CornerRadius = statusBorderRadius
                };
                Grid.SetColumn(status, 3);

                switch (staff[3])
                {
                    case "On Duty":
                        status.Background = green;
                        break;
                    case "Off Duty":
                        status.Background = gray;
                        break;
                    default:
                        status.Background = red;
                        break;
                }

                TextBlock statusText = new TextBlock()
                {
                    Text = staff[3],
                    FontSize = 19,
                    FontFamily = ubuntu,
                    Padding = pad3
                };

                status.Child = statusText;

                g.Children.Add(name);
                g.Children.Add(role);
                g.Children.Add(shift);
                g.Children.Add(status);

                b.Child = g;

                staffsPanel.Children.Add(b);
                staffsDisplayed++;
            }
        }

        private void Display_Filtered_Staffs()
        {
            staffsPanel.Children.Clear();
            int staffsDisplayed = 0;
         
            foreach (List<string> staff in filteredStaffs)
            {
                if (staffsDisplayed >= 9)
                {
                    staffsBorder.Height += 55;
                    outerMostGrid.Height += 55;
                }

                Border b = new Border()
                {
                    Height = 55,
                    BorderThickness = borderThickness,
                    BorderBrush = black
                };

                Grid g = new Grid();
                Get_Column_Definition(g);

                TextBlock name = Make_TextBlock(staff[0], pad1, 0);
                TextBlock role = Make_TextBlock(staff[1], pad2, 1);
                TextBlock shift = Make_TextBlock(staff[2], pad2, 2);

                Border status = new Border()
                {
                    Margin = statusBorderMargin,
                    CornerRadius = statusBorderRadius
                };
                Grid.SetColumn(status, 3);

                switch (staff[3])
                {
                    case "On Duty":
                        status.Background = green;
                        break;
                    case "Off Duty":
                        status.Background = gray;
                        break;
                    default:
                        status.Background = red;
                        break;
                }

                TextBlock statusText = new TextBlock()
                {
                    Text = staff[3],
                    FontSize = 19,
                    FontFamily = ubuntu,
                    Padding = pad3
                };

                status.Child = statusText;

                g.Children.Add(name);
                g.Children.Add(role);
                g.Children.Add(shift);
                g.Children.Add(status);

                b.Child = g;

                staffsPanel.Children.Add(b);
                staffsDisplayed++;
            }
        }

        private void Filter_Staffs()
        {
            filteredStaffs.Clear();

            if (roleFilterIsOn)
            {
                string role = roleFilter.Text;
                foreach (List<string> staff in staffs)
                {
                    if (staff[1] == role)
                    {
                        filteredStaffs.Add(staff);
                    }
                }

                List<List<string>> temp = new List<List<string>>(filteredStaffs);

                if (shiftFilterIsOn)
                {
                    string shift = shiftFilter.Text;
                    foreach (List<string> staff in filteredStaffs)
                    {
                        if (staff[2] != shift)
                        {
                            temp.Remove(staff);
                        }
                    }
                }

                if (statusFilterIsOn)
                {
                    string status = statusFilter.Text;
                    foreach (List<string> staff in filteredStaffs)
                    {
                        if (staff[3] != status)
                        {
                            temp.Remove(staff);
                        }
                    }
                }

                filteredStaffs = temp;
            }
            else if (shiftFilterIsOn)
            {
                string shift = shiftFilter.Text;
                foreach (List<string> staff in staffs)
                {
                    if (staff[2] == shift)
                    {
                        filteredStaffs.Add(staff);
                    }
                }

                List<List<string>> temp = new List<List<string>>(filteredStaffs);
                if (statusFilterIsOn)
                {
                    string status = statusFilter.Text;
                    foreach (List<string> staff in filteredStaffs)
                    {
                        if (staff[3] != status)
                        {
                            temp.Remove(staff);
                        }
                    }
                }
                filteredStaffs = temp;
            }
            else
            {
                string status = statusFilter.Text;
                foreach (List<string> staff in staffs)
                {
                    if (staff[3] == status)
                    {
                        filteredStaffs.Add(staff);
                    }
                }
            }
        }

        private void Display_Staff()
        {
            filterIsOn = roleFilterIsOn || shiftFilterIsOn || statusFilterIsOn;

            staffsBorder.Height = 550;
            outerMostGrid.Height = 741;
            if (filterIsOn)
            {
                Filter_Staffs();
                Display_Filtered_Staffs();
            }
            else
            {
                Display_All_Staffs();
            }
        }

        private async Task QueryStaffsTable(CancellationToken token)
        {
            SqlDataReader staffsReader = null;
            SqlCommand staffsQuery = new SqlCommand("SELECT * FROM staffs_table", MainWindow.sqlConnection);

            using (staffsReader = await staffsQuery.ExecuteReaderAsync(token))
            {
                while (staffsReader.Read())
                {
                    List<string> staff = new List<string>();
                    staff.Add(staffsReader[1].ToString()); // name
                    staff.Add(staffsReader[2].ToString()); // role
                    staff.Add(staffsReader[3].ToString()); // shift
                    staff.Add(staffsReader[4].ToString()); // status

                    staffs.Add(staff);
                }
            }

            await Dispatcher.InvokeAsync(() =>
            {
                Display_Staff();
            });

            staffsReader?.Dispose();
            staffsReader?.Close();
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
            string role = (sender as TextBlock).Text;
            roleFilter.Text = role;
            roleFilterIsOn = role == "All" ? false : true;

            showRoles.From = 138;
            showRoles.To = 0;
            rolesIsOpen = false;
            Roles.BeginAnimation(HeightProperty, showRoles);

            Display_Staff();
        }

        private bool rolesIsOpen = false;

        private void Role_DropDown_Click(object sender, MouseButtonEventArgs e)
        {
            if (rolesIsOpen)
            {
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
            string shift = (sender as TextBlock).Text;
            shiftFilter.Text = shift;
            shiftFilterIsOn = shift == "All" ? false : true;

            showShifts.From = 138;
            showShifts.To = 0;
            shiftsIsOpen = false;
            Shifts.BeginAnimation(HeightProperty, showShifts);

            Display_Staff();
        }

        private bool shiftsIsOpen = false;

        private void Shift_DropDown_Click(object sender, MouseButtonEventArgs e)
        {
            if (shiftsIsOpen)
            {
                showShifts.From = 138;
                showShifts.To = 0;
                shiftsIsOpen = false;
            }
            else
            {
                ShiftsDropDown.CornerRadius = new CornerRadius(8, 8, 0, 0);
                showShifts.From = 0;
                showShifts.To = 138;
                shiftsIsOpen = true;
            }

            Shifts.BeginAnimation(HeightProperty, showShifts, HandoffBehavior.SnapshotAndReplace);
        }

        private void Status_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string status = (sender as TextBlock).Text;
            statusFilter.Text = status;
            statusFilterIsOn = status == "All" ? false : true;

            showStatus.From = 138;
            showStatus.To = 0;
            statusIsOpen = false;
            Status.BeginAnimation(HeightProperty, showStatus);

            Display_Staff();
        }

        private bool statusIsOpen = false;

        private void Status_DropDown_Click(object sender, MouseButtonEventArgs e)
        {
            if (statusIsOpen)
            {
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
