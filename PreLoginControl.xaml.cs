using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
