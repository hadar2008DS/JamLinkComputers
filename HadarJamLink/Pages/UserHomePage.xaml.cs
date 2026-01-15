using ClientSide;
using JamLinkComputers.UControl;
using Model;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        private SideBarBTN SideBarBTNInstance;
        private Person currentUser;

        public UserHomePage(Person user)
        {
            InitializeComponent();

            //// שומרים את שם המשתמש זמנית ב־Application
            //Application.Current.Properties["username"] = user.Username;

            //// Initialize SideBarBTNInstance by finding it in the XAML
            //SideBarBTNInstance = this.FindName("SideBarBTN") as SideBarBTN;
            //if (SideBarBTNInstance != null)
            //{
            //    // Subscribe a handler that matches Action<string>
            //    SideBarBTNInstance.MenuClicked += SideBarBTN_MenuClicked;
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("SideBarBTNInstance is null - UI element missing");
            //}

            //Loaded += UserHomePage_Loaded;
            currentUser = user;

            Loaded += UserHomePage_Loaded;
        }

        //private async void UserHomePage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var username = Application.Current.Properties["username"] as string;

        //    if (string.IsNullOrEmpty(username))
        //    {
        //        MessageBox.Show("User data not found.");
        //        return;
        //    }

        //    //WelcomeText.Text = "Welcome, " + username;

        //    try
        //    {
        //        // מביאים את כל האנשים
        //        var people = await apiService.GetPeople();
        //        var currentUser = people.FirstOrDefault(p => p.Username == username);

        //        if (currentUser == null)
        //        {
        //            MessageBox.Show("User data not found.");
        //            return;
        //        }

        //        // קובעים role לפי בדיקה בטבלאות
        //        bool isMusician = await IsUserMusician(currentUser.Id);
        //        bool isProducer = await IsUserProducer(currentUser.Id);

        //        // מעדכנים Sidebar
        //        if (SideBarBTNInstance != null)
        //            SideBarBTNInstance.SetRole(isMusician, isProducer);
        //        else
        //            System.Diagnostics.Debug.WriteLine("SideBarBTNInstance is null - UI element missing");

        //        // ברירת מחדל
        //        ShowOnlyPanel(HomePanel);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Error loading user data.");
        //    }
        //}

        //// ================= ROLE CHECK =================

        //private async Task<bool> IsUserMusician(int personId)
        //{
        //    var musicians = await apiService.GetMusicians();
        //    return musicians.Any(m => m.Id == personId);
        //}

        //private async Task<bool> IsUserProducer(int personId)
        //{
        //    var producers = await apiService.GetProducers();
        //    return producers.Any(p => p.Id == personId);
        //}

        //// ================= SIDEBAR EVENT =================

        //private void SideBar_MenuClicked(string menu)
        //{
        //    switch (menu)
        //    {
        //        case "Home":
        //            ShowOnlyPanel(HomePanel);
        //            break;

        //        case "Profile":
        //            ShowOnlyPanel(ProfilePanel);
        //            break;

        //        case "Groups":
        //            ShowOnlyPanel(GroupsPanel);
        //            break;

        //        case "Musician":
        //            ShowOnlyPanel(MusicianPanel);
        //            break;

        //        case "Producer":
        //            ShowOnlyPanel(ProducerPanel);
        //            break;
        //    }
        //}

        //// Adapter matching SideBarBTN.MenuClicked (Action<string>)
        //private void SideBarBTN_MenuClicked(string menu)
        //{
        //    SideBar_MenuClicked(menu);
        //}

        //// ================= PANELS =================

        //private void ShowOnlyPanel(UIElement panelToShow)
        //{
        //    HomePanel.Visibility = Visibility.Collapsed;
        //    ProfilePanel.Visibility = Visibility.Collapsed;
        //    GroupsPanel.Visibility = Visibility.Collapsed;
        //    MusicianPanel.Visibility = Visibility.Collapsed;
        //    ProducerPanel.Visibility = Visibility.Collapsed;

        //    panelToShow.Visibility = Visibility.Visible;
        //}

        //private void MusicianBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    // TODO: Add logic for when the "Upload Segment" button is clicked
        //    // For example, show the MusicianPanel and hide others
        //    HomePanel.Visibility = Visibility.Collapsed;
        //    ProfilePanel.Visibility = Visibility.Collapsed;
        //    GroupsPanel.Visibility = Visibility.Collapsed;
        //    MusicianPanel.Visibility = Visibility.Visible;
        //    ProducerPanel.Visibility = Visibility.Collapsed;
        //}

        //private void SideBar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    // Add your logic here, or leave empty if not needed
        //}

        //private void SideBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    // Add your logic here, or leave empty if not needed
        //}
        //private void Profile_Click(object sender, RoutedEventArgs e)
        //{
        //    // Show ProfilePanel, hide others
        //    HomePanel.Visibility = Visibility.Collapsed;
        //    ProfilePanel.Visibility = Visibility.Visible;
        //    GroupsPanel.Visibility = Visibility.Collapsed;
        //    MusicianPanel.Visibility = Visibility.Collapsed;
        //    ProducerPanel.Visibility = Visibility.Collapsed;
        //}

        //private void ProducerBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    ProducerPanel.Visibility = Visibility.Collapsed;
        //}

        //private void Groups_Click(object sender, RoutedEventArgs e)
        //{

        //    //ProfilePanel.Visibility = Visibility.Visible;
        //    GroupsPanel.Visibility = Visibility.Collapsed;
        //    //MusicianPanel.Visibility = Visibility.Collapsed;

        //}

        //private void Home_Click(object sender, RoutedEventArgs e)
        //{
        //    HomePanel.Visibility = Visibility.Collapsed;
        //}

        private async void UserHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            // מציאת ה־Sidebar מתוך ה־XAML
            SideBarBTNInstance = FindName("SideBarBTN") as SideBarBTN;

            if (SideBarBTNInstance != null)
                SideBarBTNInstance.MenuClicked += SideBar_MenuClicked;

            await LoadUserRole();

            // ברירת מחדל
            LoadView(new HomeV());
        }

        // ================= ROLE LOGIC =================

        private async Task LoadUserRole()
        {
            bool isMusician = await IsUserMusician(currentUser.Id);
            bool isProducer = await IsUserProducer(currentUser.Id);

            SideBarBTNInstance?.SetRole(isMusician, isProducer);
        }

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

        // ================= NAVIGATION =================

        private void SideBar_MenuClicked(string menu)
        {
            switch (menu)
            {
                case "Home":
                    LoadView(new HomeV());
                    break;

                case "Profile":
                    LoadView(new ProfileV(currentUser));
                    break;

                case "Groups":
                    LoadView(new GroupsV());
                    break;

                case "Musician":
                    LoadView(new MusicianV());
                    break;

                case "Producer":
                    LoadView(new ProducerV(currentUser));
                    break;
            }
        }

        // ================= VIEW LOADER =================

        private void LoadView(UserControl view)
        {
            MainContent.Content = view;
        }

        private void ProducerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MusicianBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
