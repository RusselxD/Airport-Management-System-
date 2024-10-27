using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Pre_Login.xaml
    /// </summary>
    public partial class PreLoginControl : UserControl
    {
        public PreLoginControl()
        {
            InitializeComponent();
        }

        private void Proceed_To_Database_Login(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this) as LogIn_Window;
            if (window != null)
            {
                window.GoToLogin();
            }
        }
    }

}
