using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
using static System.Net.Mime.MediaTypeNames;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for GatesControl.xaml
    /// </summary>
    public partial class GatesControl : UserControl
    {
        private Dictionary<int, Border> gatesMap;
        private Dictionary<int, TextBlock> statusMap;
        private Dictionary<int, TextBlock> messageMap;




        public GatesControl(SqlConnection sqlConnection)
        {
            InitializeComponent();

            gatesMap = new Dictionary<int, Border>();
            statusMap = new Dictionary<int, TextBlock>();
            messageMap = new Dictionary<int, TextBlock>();

            Task.Run(() => QueryGatesTable(MainWindow.cts.Token));
        }

        private void Populate_Maps()
        {
            gatesMap.Add(1, t1a);
            gatesMap.Add(2, t1b);
            gatesMap.Add(3, t1c);
            gatesMap.Add(4, t1d);
            gatesMap.Add(5, t1e);
            gatesMap.Add(6, t1f);

            statusMap.Add(1, t1aStatus);
            statusMap.Add(2, t1bStatus);
            statusMap.Add(3, t1cStatus);
            statusMap.Add(4, t1dStatus);
            statusMap.Add(5, t1eStatus);
            statusMap.Add(6, t1fStatus);

            messageMap.Add(1, t1aMessage);
            messageMap.Add(2, t1bMessage);
            messageMap.Add(3, t1cMessage);
            messageMap.Add(4, t1dMessage);
            messageMap.Add(5, t1eMessage);
            messageMap.Add(6, t1fMessage);
        }

        private async Task QueryGatesTable(CancellationToken token)
        {            
            using (SqlCommand gatesQuery = new SqlCommand("SELECT * FROM gates_table", MainWindow.sqlConnection))
            {
                using (SqlDataReader gatesReader = await gatesQuery.ExecuteReaderAsync(token))
                {

                    while (gatesReader.Read())
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            





                        }).Task.Wait();

                    }
                }
            }

        }


        private void ewan()
        {

            Border gatesBorder = new Border()
            {
                CornerRadius = new CornerRadius(15),
                Margin = new Thickness(65, 0, 65, 25),
                Background = Brushes.White,
                Height = 1290
            };
            Grid.SetRow(gatesBorder, 1);

            outerGrid.Children.Add(gatesBorder);

            Grid gatesGrid = new Grid();
            gatesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gatesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            gatesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            //  gatesGrid.Children.Add(terminal1GatesBorder);

            //  Terminal 1 --------------------
            Border terminal1GatesBorder = new Border();
            Grid.SetRow(terminal1GatesBorder, 0);

            Grid terminal1GatesGrid = new Grid();
            addGrid_Row_And_Column_Definitions(terminal1GatesGrid);

            terminal1GatesGrid.Children.Add(Get_Terminal_Header("Terminal 1", 0));



            //< Border Grid.Column = "0" Grid.Row = "1" Background = "#FFBAFFBE" Margin = "31,0,15,25" CornerRadius = "8,8,8,8" >
            //                        < Grid >
            //                            < TextBlock Margin = "26,20,0,74" Text = "Gate 1A" FontFamily = "Ubuntu" FontWeight = "SemiBold" FontSize = "30" ></ TextBlock >

            //                            < TextBlock Margin = "26,66,0,53" Text = "Available" FontWeight = "Medium" FontSize = "23" FontFamily = "Ubuntu" Foreground = "#FF00AE0A" ></ TextBlock >

            //                            < TextBlock Margin = "26,107,0,15" Text = "Ready for Assignment" FontFamily = "Ubuntu" Foreground = "#FF787878" FontSize = "18" ></ TextBlock >

            //                        </ Grid >
            //                    </ Border >

            // ------------------



        }

        private TextBlock Get_Terminal_Header(string text, int row)
        {
            TextBlock newTerminalHeader = new TextBlock()
            {
                Text = text,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Ubuntu"),
                FontSize = 30,
                Padding = new Thickness(30, 25, 0, 0)
            };
            Grid.SetRow(newTerminalHeader, row);
            return newTerminalHeader;
        }

        private void addGrid_Row_And_Column_Definitions(Grid grid)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.45, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        }
    }
}
