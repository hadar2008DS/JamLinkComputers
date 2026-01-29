using Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JamLinkComputers.UControl
{
  
    /// <summary>
    /// Interaction logic for SideBarBTN.xaml
    /// </summary>
    public partial class SideBarBTN : UserControl
    {
        // A delegate is a type that represents a method
        // The method takes ONE parameter and returns NOTHING (void)
        // T means the parameter can be of any type (for example: string, int)
        public event Action<string> MenuClicked;
        // An event that is triggered when a menu item is clicked
        // Action<string> means the event sends a string and returns nothing
        // Other classes can listen to this event and react to it
        // The string can represent the menu name (for example: "Home", "Profile")

        private DispatcherTimer sidebarTimer;
        private bool isSidebarOpen = false;
        private double closedWidth = 20;   // רוחב סגור
        private double openWidth = 140;    // רוחב פתוח
        private int delayMs = 1000;        // זמן השהיה לפני סגירה (במילישניות)


        
        public SideBarBTN()
        {
            InitializeComponent();
            sidebarTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // 5 שניות
            };
            sidebarTimer.Tick += SidebarTimer_Tick;
        }

        public void SetRole(bool isMusician, bool isProducer)
        {
            if (isMusician)
                MusicianButton.Visibility = Visibility.Visible;
            else
                MusicianButton.Visibility = Visibility.Collapsed;

            // If the user is a producer – show the producer button
            if (isProducer)
                ProducerButton.Visibility = Visibility.Visible;
            else
                ProducerButton.Visibility = Visibility.Collapsed;

        }
        //explaintion what is invoke
        // Invoke is a method that calls the event
        // When we call MenuClicked.Invoke("Home"), it triggers the event
        // and any code that is listening to this event will run
        // For example, if we have a method that shows the home page,
        // it will run when we click the Home button


        //private void Settings_Click(object sender, RoutedEventArgs e)
        //    => MenuClicked.Invoke("Settings");
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MenuClicked?.Invoke("Home");
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MenuClicked?.Invoke("Profile");
        }

        private void Groups_Click(object sender, RoutedEventArgs e)
        {
            MenuClicked?.Invoke("Groups");
        }

        private void Musician_Click(object sender, RoutedEventArgs e)
        {
            MenuClicked?.Invoke("Musician");
        }

        private void Producer_Click(object sender, RoutedEventArgs e)
        {
            MenuClicked?.Invoke("Producer");
        }

        private void SideBar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                AnimateSidebar(openWidth);  // שולח את הערך הפתוח
                isSidebarOpen = true;
            }
        }

        private void SideBar_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateSidebar(closedWidth);   // שולח את הערך הסגור
            isSidebarOpen = false;
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            AnimateSidebar(closedWidth);
            //FadeText(0);
            isSidebarOpen = false;

            sidebarTimer.Stop();
        }

        // ================= ANIMATIONS =================
        private void AnimateSidebar(double targetWidth)
        {
            SideBar.BeginAnimation(WidthProperty, new DoubleAnimation
            {
                From = SideBar.ActualWidth,           // מתחיל מהרוחב הנוכחי
                To = targetWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            });
        }


        //private void FadeText(double opacity)
        //{
        //    WelcomeText.BeginAnimation(OpacityProperty, new DoubleAnimation
        //    {
        //        To = opacity,
        //        Duration = TimeSpan.FromMilliseconds(200)
        //    });
        //}
    }
}
