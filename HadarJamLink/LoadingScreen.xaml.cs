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
        //inspiration from:
        //https://learn.microsoft.com/en-us/answers/questions/1062347/how-to-show-a-loading-to-the-user-during-a-heavy-o
        //https://www.youtube.com/watch?v=c7GRu6X1zWg&t=40s. needs to be integrate it here later on
        private readonly int userId;

        public UserHome(int userId)
        {
            InitializeComponent();
            userId = userId;
            Loaded += LoadingScreen_Loaded;
        }

        private async void LoadingScreen_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // זמן טעינה
            await Task.Delay(3500);

            // ניווט למסך הבית
            NavigationService?.Navigate(new UserHomePage(userId));
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e){}
    }
}
