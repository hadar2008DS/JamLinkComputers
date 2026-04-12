using ClientSide;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HadarJamLink;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for ExploreV.xaml
    /// </summary>
    public partial class ExploreV : UserControl
    {
        private ApiService apiService = new ApiService();
        private List<Person> allUsers = new();
        public ExploreV()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            // Fetch all registered users
            var users = await apiService.GetPerson();
            if (users != null)
            {
                allUsers = users;
                UsersCardsList.ItemsSource = allUsers;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = SearchBox.Text.ToLower();
        UsersCardsList.ItemsSource = allUsers
            .Where(u => u.Username.ToLower().Contains(query))
            .ToList();
    }

        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            // 1. חילוץ המשתמש שנבחר מהכפתור
            if (sender is Button btn && btn.DataContext is Person selectedUser)
            {
                // 2. מציאת החלון הראשי בצורה מפורשת (בלי dynamic)
                var mainWindow = Window.GetWindow(this) as HadarJamLink.MainWindow;

                if (mainWindow != null)
                {
                    // 3. ניווט באמצעות ה-Frame
                    mainWindow.MainFrame.Navigate(new PublicProfileV(selectedUser));
                }
            }
        }

    //    private void SwitchToProfile(Person user)
    //{
    //    // This is a common way to find the main window and change its content
    //    var mainWindow = System.Windows.Application.Current.MainWindow as dynamic; 
    //    mainWindow.MainContentArea.Children.Clear();
    //    mainWindow.MainContentArea.Children.Add(new PublicProfileV(user));
    //}
    }
}
