using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Airport_Management_System
{
    /// <summary>
    /// Interaction logic for Landing.xaml
    /// </summary>
    public partial class Landing : Window
    {
        public Landing()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            
              AdjustAnimationDuration(TimeSpan.FromMilliseconds(1500));
        }

        // Method to adjust the animation duration programmatically
        private void AdjustAnimationDuration(TimeSpan duration)
        {
            var storyboard = new Storyboard();
            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(duration)
            };

            // Set the event when the animation completes
            fadeAnimation.Completed += AnimationCompleted;

            Storyboard.SetTarget(fadeAnimation, MyImage);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("Opacity"));

            storyboard.Children.Add(fadeAnimation);
            MyImage.BeginStoryboard(storyboard);
        }

        // Event handler when the animation completes
        private async void AnimationCompleted(object sender, EventArgs e)
        {
            await Task.Delay(2000);
            new LogIn_Window().Show();
            this.Close();
        }
    }
}
