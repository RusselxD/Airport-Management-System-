using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public string[] serverTypes { get; set; }
        public string[] authTypes { get; set; }

        private string authenticationType;
        public LoginControl()
        {
            InitializeComponent();
            serverTypes = new string[] { "Database Engine", "Analysis Services", "Reporting Services", "Integration Services" };
            authTypes = new string[] { "Windows Authentication", "SQL Server Authentication" };
            DataContext = this;

            authenticationType = "Windows Authentication";
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

            if (authenticationType is "SQL Server Authentication")
            {
                if (String.IsNullOrWhiteSpace(UsernameInput.Text))
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

        private TextBox UsernameInput;
        private PasswordBox PasswordInput;

        // This method initializes Log In Username and Password input and labels when the Server's
        // Authentication is SQL Server Authentication
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

        private int n = 0;
        private void AuthTypePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdditionalAuthControls.Children.Clear();
            authenticationType = AuthTypePicker.SelectedItem as string;

            string localAuthenticationType = authenticationType;

            if (localAuthenticationType != null)
            {
                switch (localAuthenticationType)
                {
                    // hide log in and password label and text box
                    case "Windows Authentication":

                        AdditionalAuthControls.Visibility = Visibility.Collapsed;
                        this.Height = 589;
                        
                        if (n > 0)
                        {
                            var w1 = Window.GetWindow(this) as LogIn_Window;
                            if (w1 != null)
                            {
                                w1.AdjustPosition(60);
                            }
                        }
                        n++;

                        break;

                    // show log in and password label and text box
                    case "SQL Server Authentication":

                        this.Height = 659;
                        ShowLogInAndPasswordInputs();
                        
                        var w2 = Window.GetWindow(this) as LogIn_Window;
                        if (w2 != null)
                        {
                            w2.AdjustPosition(-60);
                        }

                        break;

                }
            }
        }

        // This method is invoked when user pressed "Connect" button.
        // Validates proper inputs first before establishing a connection to the Database.
        private void Request_Server_Connection(object sender, MouseButtonEventArgs e)
        {
            if (!Valid_Input(out int n))
            {
                string errorMessage = "";
                switch (n)
                {
                    case 1:
                        errorMessage = "Please enter a Server Name.";
                        serverNameInput.Focus();
                        break;
                    case 2:
                        errorMessage = "Please enter a Database Name.";
                        databaseNameInput.Focus();
                        break;
                    case 3:
                        errorMessage = "Please enter a Log In Username.";
                        UsernameInput.Focus();
                        break;
                    case 4:
                        errorMessage = "Please enter a Password.";
                        PasswordInput.Focus();
                        break;
                }

                MessageBox.Show($"{errorMessage}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            ConnectionWindow connectionWindow = new ConnectionWindow(this);
            
            if (authenticationType is "Windows Authentication")
            {
                connectionWindow.Connect_With_Windows_Authentication(serverNameInput.Text, databaseNameInput.Text);
            }
            else
            {
                connectionWindow.Connect_With_SQL_Server_Authentication(serverNameInput.Text, databaseNameInput.Text, UsernameInput.Text, PasswordInput.Password);
            }

        }
    }
}