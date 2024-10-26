using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Security.Principal;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LandingPage : Window
    {
        public string[] serverTypes { get; set; }
        public string[] authTypes { get; set; }

        public LandingPage()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            serverTypes = new string[] { "Database Engine", "Analysis Services", "Reporting Services", "Integration Services" };
            authTypes = new string[] { "Windows Authentication", "SQL Server Authentication" };
            DataContext = this;

            currentUserLabel.Text = WindowsIdentity.GetCurrent().Name;
        }

        private bool Valid_Input(out int n)
        {
            if (String.IsNullOrWhiteSpace(serverNameInput.Text))
            {
                n = 1;
                return false;
            }

            if (String.IsNullOrWhiteSpace(databaseNameInput.Text))
            {
                n = 2;
                return false;
            }

            if(AuthTypePicker.SelectedItem as string is "SQL Server Authentication")
            {
                if(String.IsNullOrWhiteSpace(UsernameInput.Text))
                {
                    n = 3;
                    return false;
                }

                if (String.IsNullOrWhiteSpace(PasswordInput.Password))
                {
                    n = 4;
                    return false;
                }
            }

            n = 0;
            return true;
        }

        private void Connect_To_Database(object sender, RoutedEventArgs e)
        {

            if (!Valid_Input(out int n))
            {
                switch (n)
                {
                    case 1:
                        MessageBox.Show("Please enter a server name.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        serverNameInput.Focus();
                        break;
                    case 2:
                        MessageBox.Show("Please enter a database name.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        databaseNameInput.Focus();
                        break;
                    case 3:
                        MessageBox.Show("Please enter a Log In Username.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        UsernameInput.Focus();
                        break;
                    case 4:
                        MessageBox.Show("Please enter a password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        PasswordInput.Focus(); 
                        break;
                }
                return;
            }

            string connectionString = $@"Server={serverNameInput.Text}; Database={databaseNameInput.Text};User Id={UsernameInput.Text};Password={PasswordInput.Password};";
            //string connectionString = $@"Data Source={serverNameInput.Text}; Initial Catalog={databaseNameInput.Text}; Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // DESKTOP - 4CVBSIM\SQLEXPRESS
                try
                {
                    // Open the connection
                    connection.Open();
                    MessageBox.Show("Connection Successful.", "Information");
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AuthTypePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdditionalAuthControls.Children.Clear();

            string selectedAuthType = AuthTypePicker.SelectedItem as string;
            if (selectedAuthType != null)
            {
                switch (selectedAuthType)
                {
                    case "Windows Authentication":
                        AdditionalAuthControls.Visibility = Visibility.Collapsed;
                        this.Height = 589;
                        break;
                    case "SQL Server Authentication":
                        this.Height = 689;
                        ShowLogInAndPasswordInputs();
                        break;
                }

            }

        }

        private TextBox UsernameInput;
        private PasswordBox PasswordInput;

        private void ShowLogInAndPasswordInputs()
        {
            TextBlock tb1 = new TextBlock
            {
                Text = "Log In:",
                Height = 34,
                Width = 118,
                Padding = new Thickness(30, 10, 5, 5),
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 46)
            };

            UsernameInput = new TextBox
            {
                BorderThickness = new Thickness(1, 1, 1, 1),
                Height = 34,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(15, 2, 15, 0),
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 341
            };

            TextBlock tb2 = new TextBlock
            {
                Text = "Password: ",
                Height = 34,
                Width = 118,
                Padding = new Thickness(30, 10, 5, 0),
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(1, 46, 0, 0)
            };

            PasswordInput = new PasswordBox
            {
                BorderThickness = new Thickness(1, 1, 1, 1),
                Height = 34,
                Margin = new Thickness(118, 46, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(15, 2, 15, 0),
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 13
            };

            AdditionalAuthControls.Children.Add(tb1);
            AdditionalAuthControls.Children.Add(UsernameInput);
            AdditionalAuthControls.Children.Add(tb2);
            AdditionalAuthControls.Children.Add(PasswordInput);

            AdditionalAuthControls.Visibility = Visibility.Visible;

        }

    }
}
