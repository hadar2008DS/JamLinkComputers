using ClientSide;
using HadarJamLink;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace JamLinkComputers.Pages
{
    /// <summary>
    /// Interaction logic for GroupDetailsPage.xaml
    /// </summary>
    public partial class GroupDetailsPage : Page
    {
        // Static lists for the RoleConverter to access
        public static List<Model.Musician> allMusicians = new List<Model.Musician>();
        public static List<Model.Producer> allProducers = new List<Model.Producer>();
        public static List<Model.Instruments> allInstruments = new List<Model.Instruments>();
        public static List<Model.MusicianInstruments> MusicianInstrumentLinks = new List<Model.MusicianInstruments>();
        private int groupId; // assuming this is where you store the current group ID
        private ApiService apiService = new ApiService();
        public GroupDetailsPage(int groupId)
        {
            InitializeComponent();
            this.groupId = groupId;
            this.DataContext = this;
            LoadData();
        }

        private async void LoadData()
        {
            // 1. Fetching all data
            var membersResponse = await apiService.GetGroupMembers();
            var allPeopleResponse = await apiService.GetPeople();

            // Populate static lists for the Converter
            allMusicians = await apiService.GetMusicians() ?? new List<Model.Musician>();
            allProducers = await apiService.GetProducers() ?? new List<Model.Producer>();
            allInstruments = await apiService.GetInstruments() ?? new List<Model.Instruments>();
            MusicianInstrumentLinks = await apiService.GetMusicianInstruments() ?? new List<Model.MusicianInstruments>();

            // 2. Filter group members
            List<Model.Person> groupPeople = new List<Model.Person>();
            foreach (var link in membersResponse)
            {
                if (link.Group.Id == groupId)
                {
                    var person = allPeopleResponse.FirstOrDefault(p => p.Id == link.Id);
                    if (person != null) groupPeople.Add(person);
                }
            }

            // 3. Binding and Refresh
            MembersList.ItemsSource = groupPeople;

            // Force UI to re-run converters once data is loaded
            ICollectionView view = CollectionViewSource.GetDefaultView(MembersList.ItemsSource);
            view?.Refresh();

            // Update Title
            var groups = await apiService.GetGroups();
            GroupNameTitle.Text = groups.FirstOrDefault(g => g.Id == groupId)?.GroupName ?? "Group Details";
        }

        // This function will be called from the XAML to determine the role
        public string GetUserRole(int personId)
        {
            bool isMusician = false;
            bool isProducer = false;

            // Verify if the ID exists in the Musicians list
            foreach (Model.Musician m in allMusicians)
            {
                if (m.Id == personId)
                {
                    isMusician = true;
                    break;
                }
            }

            // Verify if the ID exists in the Producers list
            foreach (Model.Producer p in allProducers)
            {
                if (p.Id == personId)
                {
                    isProducer = true;
                    break;
                }
            }

            // Return the combined or specific role string
            if (isMusician && isProducer) return "Musician & Producer";
            if (isMusician) return "Musician";
            if (isProducer) return "Producer";
            return "Member";
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

    }


    public class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int personId)
            {
                // 1. Determine Roles (Your GetRoleText Logic)
                bool isMusician = GroupDetailsPage.allMusicians.Any(m => m.Id == personId);
                bool isProducer = GroupDetailsPage.allProducers.Any(p => p.Id == personId);

                string roleName = "";
                if (isMusician && isProducer) roleName = "Musician & Producer";
                else if (isMusician) roleName = "Musician";
                else if (isProducer) roleName = "Producer";
                else return "Member";

                // 2. Integration of Instruments (Your Musician Logic)
                if (isMusician)
                {
                    var myInstrumentIds = GroupDetailsPage.MusicianInstrumentLinks
                        .Where(mi => mi.Musician != null && mi.Musician.Id == personId)
                        .Select(mi => mi.Instruments.Id)
                        .ToList();

                    var myInstrumentNames = GroupDetailsPage.allInstruments
                        .Where(i => myInstrumentIds.Contains(i.Id))
                        .Select(i => i.InstrumentName)
                        .ToList();

                    if (myInstrumentNames.Any())
                    {
                        string instruments = string.Join(", ", myInstrumentNames);
                        return $"{roleName} ({instruments})";
                    }
                }

                return roleName;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
