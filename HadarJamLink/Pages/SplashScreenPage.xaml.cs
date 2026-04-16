using HadarJamLink;
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
using System.Windows.Threading;

namespace JamLinkComputers.Pages
{
    /// <summary>
    /// Interaction logic for SplashScreenPage.xaml
    /// </summary>
    public partial class SplashScreenPage : Page
    {
        private DispatcherTimer timer;
        public SplashScreenPage()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3); // מחכה 3 שניות
            timer.Tick += (s, e) => {
                timer.Stop();
                // ניווט לדף הכניסה בתוך אותו Frame
                this.NavigationService.Navigate(new LogInPage());
            };
            timer.Start();
        }
    }
}
