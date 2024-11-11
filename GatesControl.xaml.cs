using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for GatesControl.xaml
    /// </summary>
    public partial class GatesControl : UserControl
    {
        public GatesControl(SqlConnection sqlConnection)
        {
            InitializeComponent();

            Task.Run(() => QueryGatesTable(MainWindow.cts.Token));
        }

        private async Task QueryGatesTable(CancellationToken token)
        {            
            using (SqlCommand gatesQuery = new SqlCommand("SELECT * FROM gates_table", MainWindow.sqlConnection))
            {
                using (SqlDataReader gatesReader = await gatesQuery.ExecuteReaderAsync(token))
                {
                    int row = 1;



                    while (gatesReader.Read())
                    {


                        Dispatcher.InvokeAsync(() =>
                        {
                            //  AddAlert(alertID, alertMessage, alertCode);
                        }).Task.Wait();

                    }
                }
            }
            


        }
    }
}
