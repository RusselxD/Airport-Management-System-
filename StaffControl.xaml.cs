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
using static System.Windows.Controls.Image;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Imaging;

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

        DoubleAnimation showRoles;
        DoubleAnimation showShifts;
        DoubleAnimation showStatus;

        // -------------------------------------- STAFF BORDER REUSABLE COMPONENTS -------------------------------------- //
        private Thickness borderThickness = new Thickness(0, 0, 0, 1);
        private Brush black = Brushes.Black;

        private Thickness pad1 = new Thickness(20, 18, 0, 0);
        private Thickness pad2 = new Thickness(0, 18, 0, 0);
        private Thickness pad3 = new Thickness(10, 13, 0, 0);

        private Thickness statusBorderMargin = new Thickness(0, 5, 5, 5);
        private CornerRadius statusBorderRadius = new CornerRadius(5);

        FontFamily ubuntu = new FontFamily("Ubuntu");

        private SolidColorBrush green = new SolidColorBrush(Color.FromRgb(186, 255, 167));
        private SolidColorBrush gray = new SolidColorBrush(Color.FromRgb(191, 191, 191));
        private SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 192, 192));

        BitmapImage edit = new BitmapImage(new Uri("/Icons/staff-edit.png", UriKind.Relative));
        BitmapImage delete = new BitmapImage(new Uri("/Icons/staff-delete.png", UriKind.Relative));
        BitmapImage details = new BitmapImage(new Uri("/Icons/staff-details.png", UriKind.Relative));
        Thickness imageMargin = new Thickness(2);

        Style hoverStyle = new Style(typeof(Border));

        Thickness t2 = new Thickness(0, 0, 20, 0);
        Thickness t1 = new Thickness(20, 0, 0, 0);
        CornerRadius c1 = new CornerRadius(4);

        // --------------------------------------------------------------------------------------------------------------- //

        private HomePageControl homePage;

        TextBlock[] dropDownChoices;

        public StaffControl(HomePageControl homePage)
        {
            this.homePage = homePage;
            roleFilterIsOn = false;
            shiftFilterIsOn = false;
            statusFilterIsOn = false;

            filterIsOn = roleFilterIsOn || shiftFilterIsOn || statusFilterIsOn;

            InitializeComponent();

            dropDownChoices = new TextBlock[]
            {
                all1, role1, role2, role3, 
                role4, role5, role6, role7, 
                role8, role9, role10, role11, 
                role12, role13, role14, role15, 
                role16, role17, role18, role19, 
                role20, role21, all2, shift1, shift2,
                shift3, all3, status1, status2, status3, status4
            };

            Refresh();
            InitializeAnimations();
        }

        public void Refresh()
        {
            staffs = new List<List<string>>();
            filteredStaffs = new List<List<string>>();

            Task.Run(() => QueryStaffsTable(MainWindow.cts.Token));
        }

        private void InitializeAnimations()
        {
            Duration d = new Duration(TimeSpan.FromMilliseconds(300));
            showRoles = new DoubleAnimation
            {
                Duration = d
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
                Duration = d
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
                Duration = d
            };
            showStatus.Completed += (s, args) =>
            {
                if (!statusIsOpen)
                {
                    StatusDropDown.CornerRadius = new CornerRadius(8);
                }
            };

            hoverStyle = new Style(typeof(Border));
            hoverStyle.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.White));
            Trigger isMouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            isMouseOverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(235, 235, 235))));
            hoverStyle.Triggers.Add(isMouseOverTrigger);

            Style dropDownChoiceHoverEffect = new Style(typeof(TextBlock));
            Trigger mouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            mouseOverTrigger.Setters.Add(new Setter
            {
                Property = TextBlock.BackgroundProperty,
                Value = new SolidColorBrush(Color.FromRgb(233, 233, 233)) 
            });
            dropDownChoiceHoverEffect.Triggers.Add(mouseOverTrigger);
            foreach (TextBlock tb in dropDownChoices)
            {
                tb.Style = dropDownChoiceHoverEffect;
            }
        }

        private void Get_Column_Definition(Grid g)
        {
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.15, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) });
        }

        private TextBlock Make_TextBlock(string text, Thickness pad, int col)
        {
            TextBlock tb = new TextBlock()
            {
                Text = text,
                FontSize = 17,
                FontFamily = ubuntu,
                Padding = pad
            };
            Grid.SetColumn(tb, col);

            return tb;
        }

        private async Task Display_Staffs()
        {
            try
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    staffsPanel.Children.Clear();
                });

                int staffsDisplayed = 0;

                List<List<string>> staffsToBeDisplayed = filterIsOn ? filteredStaffs : staffs;

                foreach (List<string> staff in staffsToBeDisplayed)
                {
                    if (staffsDisplayed >= 9)
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            staffsBorder.Height += 55;
                            outerMostGrid.Height += 55;
                        });
                    }

                    Border s = await Dispatcher.InvokeAsync(() =>
                    {
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
                            FontSize = 17,
                            FontFamily = ubuntu,
                            Padding = pad3
                        };

                        status.Child = statusText;

                        Border tools = new Border();
                        Grid toolsGrid = new Grid();
                        tools.Child = toolsGrid;
                        Grid.SetColumn(tools, 4);

                        Border editBorder = new Border()
                        {
                            Name = staff[0].Replace(' ', '_'),
                            Cursor = Cursors.Hand,
                            Width = 30,
                            Height = 30,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = t1,
                            CornerRadius = c1,
                        };
                        editBorder.Style = hoverStyle;

                        editBorder.MouseDown += (ss, e) =>
                        {
                            Edit_Staff(ss, e);
                        };

                        System.Windows.Controls.Image editImage = new System.Windows.Controls.Image
                        {
                            Source = edit,
                            Margin = imageMargin
                        };
                        editBorder.Child = editImage;

                        Border deleteBorder = new Border()
                        {
                            Name = staff[0].Replace(' ', '_'),
                            Cursor = Cursors.Hand,
                            Width = 30,
                            Height = 30,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            CornerRadius = c1,
                        };

                        deleteBorder.MouseDown += (ss, e) =>
                        {
                            Delete_Staff(ss, e);
                        };

                        deleteBorder.Style = hoverStyle;

                        System.Windows.Controls.Image deleteImage = new System.Windows.Controls.Image
                        {
                            Source = delete,
                            Margin = imageMargin
                        };

                        deleteBorder.Child = deleteImage;

                        Border detailsBorder = new Border()
                        {
                            Name = staff[0].Replace(' ', '_'),
                            Cursor = Cursors.Hand,
                            Width = 30,
                            Height = 30,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = t2,
                            CornerRadius = c1,
                        };

                        detailsBorder.MouseDown += (ss, e) =>
                        {
                            Show_Details(ss, e);
                        };

                        detailsBorder.Style = hoverStyle;

                        System.Windows.Controls.Image detailsImage = new System.Windows.Controls.Image
                        {
                            Source = details,
                            Margin = imageMargin
                        };

                        detailsBorder.Child = detailsImage;

                        toolsGrid.Children.Add(editBorder);
                        toolsGrid.Children.Add(deleteBorder);
                        toolsGrid.Children.Add(detailsBorder);

                        g.Children.Add(name);
                        g.Children.Add(role);
                        g.Children.Add(shift);
                        g.Children.Add(status);
                        g.Children.Add(tools);

                        b.Child = g;

                        return b;
                    });

                    await Dispatcher.InvokeAsync(() =>
                    {
                        staffsPanel.Children.Add(s);
                    });

                    staffsDisplayed++;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private async void Edit_Staff(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as Border).Name.Replace('_', ' ');
            List<string> details = await Task.Run(() => Query_Staff_Details(name));

            if (details == null)
            {
                return;
            }

            await Dispatcher.InvokeAsync(() =>
            {
                new EditStaffDetails(details, this);
            });
        }

        private async Task<List<string>> Query_Staff_Details(string name)
        {
            List<string> details = new List<string>();

            try
            {
                await Task.Run(async () =>
                {
                    using (SqlCommand findStaffQuery = new SqlCommand("SELECT * FROM staffs_table WHERE name_col = @Name", MainWindow.sqlConnection))
                    {
                        findStaffQuery.Parameters.AddWithValue("@Name", name);
                        using (SqlDataReader findStaffReader = await findStaffQuery.ExecuteReaderAsync())
                        {
                            while (await findStaffReader.ReadAsync())
                            {
                                details.Add(findStaffReader[1].ToString());
                                details.Add(findStaffReader[2].ToString());
                                details.Add(findStaffReader[3].ToString());
                                details.Add(findStaffReader[4].ToString());
                                details.Add(findStaffReader[5].ToString());
                                details.Add(findStaffReader[6].ToString());
                                details.Add(findStaffReader[7].ToString());
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            return details;
        }

        private async void Show_Details(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as Border).Name.Replace('_', ' ');
            List<string> details = await Task.Run(() => Query_Staff_Details(name));

            if (details == null)
            {
                return;
            }

            await Dispatcher.InvokeAsync(() =>
            {
                Open_Staff_Details(details);
            });
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

        private async Task Display_Staff()
        {
            filterIsOn = roleFilterIsOn || shiftFilterIsOn || statusFilterIsOn;

            await Dispatcher.InvokeAsync(() =>
            {
                staffsBorder.Height = 550;
                outerMostGrid.Height = 741;
            });

            if (filterIsOn)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Filter_Staffs();
                });
            }
            await Task.Run(() => Display_Staffs());
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

            await Task.Run(() => Display_Staff());
        }

        private async void Search_Icon_Click(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                MessageBox.Show("Please enter a staff name to search for.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<string> details = new List<string>();

            try
            {
                await Task.Run(async () =>
                {
                    string name = "";
                    await Dispatcher.InvokeAsync(() =>
                    {
                        name = searchBox.Text.ToUpper();
                    });

                    using (SqlCommand findStaffQuery = new SqlCommand("SELECT * FROM staffs_table WHERE name_col = @Name", MainWindow.sqlConnection))
                    {
                        findStaffQuery.Parameters.AddWithValue("@Name", name);
                        using (SqlDataReader findStaffReader = await findStaffQuery.ExecuteReaderAsync())
                        {
                            while (await findStaffReader.ReadAsync())
                            {
                                details.Add(findStaffReader[1].ToString());
                                details.Add(findStaffReader[2].ToString());
                                details.Add(findStaffReader[3].ToString());
                                details.Add(findStaffReader[4].ToString());
                                details.Add(findStaffReader[5].ToString());
                                details.Add(findStaffReader[6].ToString());
                                details.Add(findStaffReader[7].ToString());
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (details.Count == 0)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("No Staff Found. \nEnter the full name.", "No Result", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            else
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Open_Staff_Details(details);
                });
            }
        }

        private void Open_Staff_Details(List<string> details)
        {
            new StaffDetailsWindow(details);
        }

        private async void Role_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string role = (sender as TextBlock).Text;
            roleFilter.Text = role;
            roleFilterIsOn = role == "All" ? false : true;

            showRoles.From = 138;
            showRoles.To = 0;
            rolesIsOpen = false;
            Roles.BeginAnimation(HeightProperty, showRoles);

            await Task.Run(() => Display_Staff());
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

        private async void Shift_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string shift = (sender as TextBlock).Text;
            shiftFilter.Text = shift;
            shiftFilterIsOn = shift == "All" ? false : true;

            showShifts.From = 138;
            showShifts.To = 0;
            shiftsIsOpen = false;
            Shifts.BeginAnimation(HeightProperty, showShifts);

            await Task.Run(() => Display_Staff());
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

        private async void Status_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string status = (sender as TextBlock).Text;
            statusFilter.Text = status;
            statusFilterIsOn = status == "All" ? false : true;

            showStatus.From = 138;
            showStatus.To = 0;
            statusIsOpen = false;
            Status.BeginAnimation(HeightProperty, showStatus);

            await Task.Run(() => Display_Staff());
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

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!searchBoxBorder.IsMouseOver)
            {
                Keyboard.ClearFocus();
            }
        }

        private async void Delete_Staff(object s, MouseButtonEventArgs e)
        {
            string name = (s as Border).Name.Replace('_', ' ');
            if (ConfirmDelete(name))
            {
                if (await Task.Run(() => DeleteStaffQuery(name)))
                {
                    await Task.Run(() => homePage.RefreshStats(MainWindow.cts.Token));
                    homePage.addRecentAct($"Deleted {name} from Staff list.");
                    Refresh();
                    MessageBox.Show($"{name} has been successfully deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("An error occurred while deleting the staff.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ConfirmDelete(string staffName)
        {
            string message = $"Are you sure you want to delete {staffName}?";
            string caption = "Confirm Delete";
            MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            return result == MessageBoxResult.Yes;
        }

        private async Task<bool> DeleteStaffQuery(string staffName)
        {
            string query = "DELETE FROM staffs_table WHERE name_col = @StaffName";
            try
            {
                using (SqlCommand command = new SqlCommand(query, MainWindow.sqlConnection))
                {
                    command.Parameters.AddWithValue("@StaffName", staffName);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        public void UpdateHomePage(string updateMessage)
        {
            homePage.addRecentAct(updateMessage);
            Task.Run(() => homePage.RefreshStats(MainWindow.cts.Token));
        }
    }
}