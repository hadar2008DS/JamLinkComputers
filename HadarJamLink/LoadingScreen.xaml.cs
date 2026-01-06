using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace JamLinkComputers
{
    /// <summary>
    /// Interaction logic for UserHome.xaml
    /// </summary>
    public partial class UserHome : Page
    {
        private CancellationTokenSource? _cts;

        public UserHome()
        {
            InitializeComponent();
            Loaded += LoadingScreen_Loaded;
        }
        private async void LoadingScreen_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // זמן טעינה
            await Task.Delay(3500);

            // ניווט למסך הבית
            NavigationService?.Navigate(new UserHomePage());
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e){}
    }
}
