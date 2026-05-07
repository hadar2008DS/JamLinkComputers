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
        private Person currentUser;
        private HashSet<int> producerIds = new();
        private HashSet<int> musicianIds = new();
        public ExploreV(Person loggedInUser)
        {
            InitializeComponent();
            this.currentUser = loggedInUser;
            // now controls are assigned
            AllFilter.IsChecked = true; // will fire FilterUsers now that all fields exist
            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                // 1. שליפת כל הנתונים הדרושים לסיווג
                var users = await apiService.GetPerson();
                var apps = await apiService.GetProducerApps();
                var segments = await apiService.GetMusicalSegments();

                if (users != null)
                {
                    allUsers = users;

                    // 2. סיווג מפיקים: כל מי שיש לו אפליקציה ברשימת ה-Apps
                    producerIds = new HashSet<int>(apps.Select(a => a.Producer.Id));

                    // 3. סיווג מוזיקאים: כל מי שיש לו סגמנט מוזיקלי
                    musicianIds = new HashSet<int>(segments.Select(s => s.Musician.Id));

                    UsersCardsList.ItemsSource = allUsers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error classifying users: " + ex.Message);
            }
        }

        private void FilterUsers(object sender, EventArgs e)
        {
            // בדיקה שכל האלמנטים שנוצרו ב-XAML כבר קיימים בזיכרון
            // הוספנו כאן בדיקה ל-MusicianFilter ו-ProducerFilter
            if (allUsers == null || SearchBox == null || MusicianFilter == null || ProducerFilter == null)
                return;

            string query = SearchBox.Text.ToLower();

            // סינון ראשוני לפי שם
            var filtered = allUsers.Where(u => u.Username.ToLower().Contains(query));

            // סינון לפי הסיווג
            if (MusicianFilter.IsChecked == true)
            {
                filtered = filtered.Where(u => musicianIds.Contains(u.Id));
            }
            else if (ProducerFilter.IsChecked == true)
            {
                filtered = filtered.Where(u => producerIds.Contains(u.Id));
            }

            UsersCardsList.ItemsSource = filtered.ToList();
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
                // 2. מציאת החלון הראשי בצורה מפורשת בלי dynamic
                var mainWindow = Window.GetWindow(this) as HadarJamLink.MainWindow;

                // Prefer using the app's MainFrame if available
                if (mainWindow?.MainFrame != null)
                {
                    // 3. ניווט באמצעות ה Frame
                    mainWindow.MainFrame.Navigate(new PublicProfileV(selectedUser, this.currentUser));
                    return;
                }

                // Fallback: try navigation service from the control
                var nav = NavigationService.GetNavigationService(this);
                if (nav != null)
                {
                    nav.Navigate(new PublicProfileV(selectedUser, this.currentUser));
                    return;
                }

                // If both are missing, inform or log (avoid throwing)
                MessageBox.Show("Navigation target not found. Cannot open profile.");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as HadarJamLink.MainWindow;

            if (mainWindow?.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new UserHomePage(this.currentUser));
                return;
            }

            var nav = NavigationService.GetNavigationService(this);
            if (nav != null)
            {
                nav.Navigate(new UserHomePage(this.currentUser));
                return;
            }

            MessageBox.Show("Navigation target not found. Cannot go back.");
        }
    }
}
