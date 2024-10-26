﻿using System;
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
    public partial class DatabaseLogin : Window
    {
        public string[] serverTypes { get; set; }
        public string[] authTypes { get; set; }

        private string authenticationType;

        public DatabaseLogin()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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

            if(authenticationType is "SQL Server Authentication")
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

        private void Connect_To_Database()
        {
            string connectionString = "";

            if (authenticationType is "Windows Authentication")
            {
                connectionString = $@"Server={serverNameInput.Text}; Database={databaseNameInput.Text}; Trusted_Connection=True;";
            }
            else
            {
                connectionString = $@"Server={serverNameInput.Text}; Database={databaseNameInput.Text};User Id={UsernameInput.Text};Password={PasswordInput.Password};";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
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

        // This method is invoked when user pressed "Connect" button.
        // Validates proper inputs first before establishing a connection to the Database.
        private void Request_Server_Connection(object sender, RoutedEventArgs e)
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
            } 
            else
            {
                Connect_To_Database();
            }
        }

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
                        break;

                    // show log in and password label and text box
                    case "SQL Server Authentication":
                        this.Height = 689;
                        ShowLogInAndPasswordInputs();
                        break;
                }
            }
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
    }
}
