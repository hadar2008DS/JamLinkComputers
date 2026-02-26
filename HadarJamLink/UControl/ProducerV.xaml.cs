using ClientSide;
using HadarJamLink;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private async void ProducerV_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadSegments();
        }
        private async Task LoadSegments()
        {
            if (currentUser == null)
                return;

            try
            {
                var allSegments = await apiService.GetMusicalSegments();

                var producerSegments = allSegments.Where(s => s.Musician != null && s.Musician.Id == currentUser.Id).ToList();

                GenerateSegmentModules(producerSegments);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading segments: " + ex.Message);
            }
        }

        // Logic to launch apps based on the button "Tag"
        private void LaunchApp_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            string app = btn.Tag?.ToString();

            if (string.IsNullOrEmpty(app)) return;

            try
            {
                Process.Start(new ProcessStartInfo(app) { UseShellExecute = true });
            }
            catch
            {
                MessageBox.Show($"Could not find {app}. Point the 'Tag' to the full .exe path on your PC.");
            }
        }

        private void LaunchUrl_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            string url = btn.Tag.ToString();
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void GenerateSegmentModules(List<MusicalSegments> segments)
        {
            //SegmentsPanel.Children.Clear();

            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];

                Border border = new Border();
                border.Style = (Style)FindResource("SegmentModule");
                border.Width = 380;

                StackPanel stack = new StackPanel();

                TextBlock title = new TextBlock();
                title.Text = segment.SegmentName.ToUpper();
                title.Foreground = Brushes.Cyan;
                title.FontWeight = FontWeights.Bold;

                TextBlock genre = new TextBlock();
                genre.Text = "Genre: " + segment.Genre;
                genre.Foreground = Brushes.Gray;

                TextBlock duration = new TextBlock();
                duration.Text = "Duration: " + segment.Lengthinseconds;
                duration.Foreground = Brushes.Gray;

                stack.Children.Add(title);
                stack.Children.Add(genre);
                stack.Children.Add(duration);

                border.Child = stack;

                SegmentsPanel.Children.Add(border);
            }
        }
        //        private async void ProducerV_Loaded(object s, RoutedEventArgs e) => await LoadData();

        //        public async Task<List<MusicalSegments>> GetMusicalSegments(int musicianId)
        //        {
        //            var allSegments = await apiService.GetMusicalSegments(); // Use apiService to fetch segments
        //            return allSegments;
        //        }
        //        //public async Task<List<App>> GetProducerApps(int producerId)
        //        //{
        //        //    var allApps = await apiService.GetProducerApps(); // Use apiService to fetch apps
        //        //    return allApps;
        //        //}

        //        private async Task LoadData()
        //        {
        //            if (currentUser == null)
        //                return;

        ///*            AppsList apps = await apiService.GetProducerApps();*/
        //            MusicalSegmentsList segments = await apiService.GetMusicalSegments();

        //            //AppsList.ItemsSource = apps;
        //            MusicalSegments.ItemsSource = segments;
        //        }

        //        private void CreateProject_Click(object sender, RoutedEventArgs e)
        //        {
        //            MessageBox.Show("Project creation screen will be added later.");
        //        }
    }
}
