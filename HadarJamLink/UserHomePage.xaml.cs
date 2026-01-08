using ClientSide;
using Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JamLinkComputers
{
    /// <summary>
    /// Interaction logic for UserHomePage.xaml
    /// </summary>
    public partial class UserHomePage : Page
    {
        ApiService apiService = new ApiService();

        public UserHomePage(int userId)
        {
            InitializeComponent();
            Application.Current.Properties["userId"] = userId;
            Loaded += UserHomePage_Loaded;
        }

        private async void UserHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            int userId = (int)(Application.Current.Properties["userId"] ?? 0);

            try
            {
                // קבל את רשימת כל המשתמשים
                var people = await apiService.GetPeople();
                var currentUser = people?.FirstOrDefault(u => u.Id == userId);

                if (currentUser == null)
                {
                    MessageBox.Show("User data not found.");
                    return;
                }

                // הראה שם
                WelcomeText.Text = "Welcome, " + currentUser.Username;

                // קבל רשימות של מוזיקאים ומפיקים
                var musicians = await apiService.GetMusicians(); // list of Musician objects
                var producers = await apiService.GetProducers(); // list of Producer objects

                // קבע את השדות בהתאם לרשימות
                bool IsMusician = musicians.Any(m => m.Id == userId);
                bool IsProducer = producers.Any(p => p.Id == userId);

                // הצג/הסתר כפתורים לפי סוג
                MusicianButton.Visibility = IsMusician ? Visibility.Visible : Visibility.Collapsed;
                ProducerButton.Visibility = IsProducer ? Visibility.Visible : Visibility.Collapsed;

                // הצג את הפאנל הראשי
                ShowOnlyPanel(HomePanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching user data: " + ex.Message);
            }
        }

        private void ShowOnlyPanel(UIElement panelToShow)
        {
            HomePanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;
            GroupsPanel.Visibility = Visibility.Collapsed;
            MusicianPanel.Visibility = Visibility.Collapsed;
            ProducerPanel.Visibility = Visibility.Collapsed;

            panelToShow.Visibility = Visibility.Visible;
        }

        private void Home_Click(object sender, RoutedEventArgs e) => ShowOnlyPanel(HomePanel);
        private void Profile_Click(object sender, RoutedEventArgs e) => ShowOnlyPanel(ProfilePanel);
        private void Groups_Click(object sender, RoutedEventArgs e) => ShowOnlyPanel(GroupsPanel);
        private void MusicianBtn_Click(object sender, RoutedEventArgs e) => ShowOnlyPanel(MusicianPanel);
        private void ProducerBtn_Click(object sender, RoutedEventArgs e) => ShowOnlyPanel(ProducerPanel);
    }


}
