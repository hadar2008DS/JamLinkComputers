using ClientSide;
using HadarJamLink;
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

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for ProducerV.xaml
    /// </summary>
    public partial class ProducerV : UserControl
    {
        private Person currentUser;
        private ApiService apiService = new ApiService();

        public ProducerV(Person user)
        {
            currentUser = user;
            InitializeComponent();
            Loaded += ProducerV_Loaded;
        }
        private async void ProducerV_Loaded(object s, RoutedEventArgs e) => await LoadData();

        public async Task<List<MusicalSegments>> GetMusicalSegments(int musicianId)
        {
            var allSegments = await apiService.GetMusicalSegments(); // Use apiService to fetch segments
            return allSegments;
        }
        //public async Task<List<App>> GetProducerApps(int producerId)
        //{
        //    var allApps = await apiService.GetProducerApps(); // Use apiService to fetch apps
        //    return allApps;
        //}

        private async Task LoadData()
        {
            if (currentUser == null)
                return;

/*            AppsList apps = await apiService.GetProducerApps();*/
            MusicalSegmentsList segments = await apiService.GetMusicalSegments();

            //AppsList.ItemsSource = apps;
            MusicalSegments.ItemsSource = segments;
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Project creation screen will be added later.");
        }
    }
}
