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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for LogIn_Window.xaml
    /// </summary>
    public partial class LogIn_Window : Window
    {
        public LogIn_Window()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainContent.Content = new PreLoginControl();
        }

        public void GoToLogin()
        {
            this.SizeToContent = SizeToContent.Height;
            MainContent.Content = new LoginControl();
        }

        public void AdjustPosition(int n)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = this.Top,               
                To = this.Top + n,             
                Duration = TimeSpan.FromSeconds(0.15), 
            };

            this.BeginAnimation(Window.TopProperty, animation);
        }
    }
}