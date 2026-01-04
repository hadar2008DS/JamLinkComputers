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

namespace JamLinkComputers
{
    /// <summary>
    /// Interaction logic for UserHome.xaml
    /// </summary>
    public partial class UserHome : Page
    {
        public UserHome()
        {
            InitializeComponent();
            Loaded += LoadingScreen_Loaded;
        }
        private async void LoadingScreen_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // זמן טעינה
            await Task.Delay(2500);

            // ניווט למסך הבית
            NavigationService?.Navigate(new UserHomePage());
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e){}
    }
}
