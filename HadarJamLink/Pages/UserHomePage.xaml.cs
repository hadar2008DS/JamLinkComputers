using ClientSide;
using Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JamLinkComputers
{
    /// <summary>
    /// Interaction logic for UserHomePage.xaml
    /// </summary>
    public partial class UserHomePage : Page
    {
        ApiService apiService = new ApiService();

        public UserHomePage(Person user)
        {
            InitializeComponent();

            // שומרים את שם המשתמש זמנית ב־Application
            Application.Current.Properties["username"] = user.Username;
            
            Loaded += UserHomePage_Loaded;
        }

        private async void UserHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            string username =
                Application.Current.Properties["username"] as string ?? "";

            if (username == "")
            {
                MessageBox.Show("Username not found.");
                return;
            }

            WelcomeText.Text = "Welcome, " + username;

            try
            {
                // 1. הבאת כל המשתמשים
                var people = await apiService.GetPeople();
                var user = people.FirstOrDefault(p => p.Username == username);

                if (user == null)
                {
                    MessageBox.Show("User data not found.");
                    return;
                }

                // 2. קביעת role
                bool isMusician = await IsUserMusician(user.Id);
                bool isProducer = await IsUserProducer(user.Id);

                // 3. התאמת ה־UI
                MusicianButton.Visibility =
                    isMusician ? Visibility.Visible : Visibility.Collapsed;

                ProducerButton.Visibility =
                    isProducer ? Visibility.Visible : Visibility.Collapsed;

                // 4. ברירת מחדל
                ShowOnlyPanel(HomePanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading home page: " + ex.Message);
            }
        }

        // =========================
        // ROLE CHECK FUNCTIONS
        // =========================

        private async Task<bool> IsUserMusician(int personId)
        {
            var musicians = await apiService.GetMusicians();
            return musicians.Any(m => m.Id == personId);
        }

        private async Task<bool> IsUserProducer(int personId)
        {
            var producers = await apiService.GetProducers();
            return producers.Any(p => p.Id == personId);
        }

        // =========================
        // SIDEBAR PANELS
        // =========================

        private void ShowOnlyPanel(UIElement panelToShow)
        {
            HomePanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;
            GroupsPanel.Visibility = Visibility.Collapsed;
            MusicianPanel.Visibility = Visibility.Collapsed;
            ProducerPanel.Visibility = Visibility.Collapsed;

            panelToShow.Visibility = Visibility.Visible;
        }

        // =========================
        // NAVIGATION BUTTONS
        // =========================

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyPanel(HomePanel);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyPanel(ProfilePanel);
        }

        private void Groups_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyPanel(GroupsPanel);
        }

        private void MusicianBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyPanel(MusicianPanel);
        }

        private void ProducerBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyPanel(ProducerPanel);
        }

        // =========================
        // SIDEBAR OPEN / CLOSE
        // =========================

        private void SideBar_MouseEnter(object sender, MouseEventArgs e)
        {
            SideBar.Width = 140;
            SideBar.Padding = new Thickness(16);
        }

        private void SideBar_MouseLeave(object sender, MouseEventArgs e)
        {
            SideBar.Width = 52;
            SideBar.Padding = new Thickness(8);
        }
    }



}
