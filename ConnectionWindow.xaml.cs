﻿using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using Airport_Management_System.Properties;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        private LoginControl lc;
        private CancellationTokenSource cts;

        public ConnectionWindow(LoginControl lc)
        {
            // DESKTOP-4CVBSIM\SQLEXPRESS
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Show();
            this.lc = lc;

            cts = new CancellationTokenSource();

            this.Closing += (s, e) =>
            {
                cts.Cancel();
            };
        }

        private void Open_Main_Window(SqlConnection connection)
        {
            Window.GetWindow(lc).Close();
            new MainWindow(connection);
        }

        // for windows authentication
        public void Connect_With_Windows_Authentication(string serverName, string databaseName)
        {
            this.connectionString = $@"Server={serverName}; Database={databaseName}; Trusted_Connection=True;MultipleActiveResultSets = True;";
            Connect_To_Database();
        }

        // for sql server authentication
        public void Connect_With_SQL_Server_Authentication(string serverName, string databaseName, string userName, string password)
        {
            this.connectionString = $@"Server={serverName}; Database={databaseName};User Id={userName};Password={password};MultipleActiveResultSets = True;";
            Connect_To_Database();
        }

        private string connectionString;

        private void Connect_To_Database()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, e) => Worker_DoWork(sender, e, cts.Token);
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();

            worker.RunWorkerCompleted += (s, e) => worker.Dispose();
        }

        private string[] loadingMessages =
        {
            "Initializing connection with the database...",
            "Verifying system compatibility...",
            "Checking network and server availability...",
            "Establishing secure connection, please hold...",
            "Authenticating user access permissions...",
            "Preparing database queries...",
            "Retrieving essential airport data...",
            "Loading latest flight schedules...",
            "Synchronizing data with the system...",
            "Connection established! Loading complete..."
        };

        private void Worker_DoWork(object sender, DoWorkEventArgs e, CancellationToken token)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 0; i <= 10; i++)
            {
                Thread.Sleep(50);
                worker.ReportProgress(i, "Initializing connection with the database...");
            }

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                // Open the connection
                connection.Open();

                Random rand = new Random();
                for (int i = 10; i <= 99; i++)
                {
                    if (token.IsCancellationRequested) return;
                    Thread.Sleep(rand.Next(1, 30));
                    worker.ReportProgress(i, loadingMessages[i / 10]);
                }
                 Dispatcher.BeginInvoke(new Action(() => Open_Main_Window(connection)));

            }
            catch (Exception ex)
            {
                if (token.IsCancellationRequested) return;
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
            }

        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            textBlock.Text = (string)e.UserState;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 100;
            this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
        }
    }
}