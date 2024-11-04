using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        private LoginControl lc;

        public ConnectionWindow(LoginControl lc)
        {
            // DESKTOP-4CVBSIM\SQLEXPRESS
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Show();
            this.lc = lc;
        }

        private void Open_Main_Window()
        {
            Window.GetWindow(this.lc).Close();
            MainWindow mw = new MainWindow();
        }

        // for windows authentication
        public void Connect_With_Windows_Authentication(string serverName, string databaseName)
        {
            this.connectionString = $@"Server={serverName}; Database={databaseName}; Trusted_Connection=True;";
            Connect_To_Database();
        }

        // for sql server authentication
        public void Connect_With_SQL_Server_Authentication(string serverName, string databaseName, string userName, string password)
        {
            this.connectionString = $@"Server={serverName}; Database={databaseName};User Id={userName};Password={password};";
            Connect_To_Database();
        }

        private string connectionString;

        private void Connect_To_Database( )
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
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

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            for(int i = 0; i <= 10; i++)
            {
                Thread.Sleep(50);
                worker.ReportProgress(i, "Initializing connection with the database...");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    for(int i = 10; i <= 99; i++)
                    {
                        Thread.Sleep(10);
                        worker.ReportProgress(i, loadingMessages[i / 10]);
                    }
                    this.Dispatcher.BeginInvoke(new Action(() => Open_Main_Window()));

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            this.Dispatcher.BeginInvoke(new Action(() => this.Close()));

        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            textBlock.Text = (string)e.UserState;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 100;
        }
    }
}