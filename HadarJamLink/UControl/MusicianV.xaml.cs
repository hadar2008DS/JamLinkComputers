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

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for MusicianV.xaml
    /// </summary>
    public partial class MusicianV : UserControl
    {
        private Person currentUser;
        private ApiService apiService = new ApiService();
        
        public MusicianV(Person loggedInUser)
        {
            InitializeComponent();
            this.currentUser = loggedInUser;
            LoadMusicianData();
        }

        private async void LoadMusicianData()
        {
            try
            {
                // 1. Fetch all musical segments from the API
                var allSegments = await apiService.GetMusicalSegments();
                if (allSegments == null) return;

                // 2. Filter segments for the current musician using Id_musician
                var mySegments = allSegments
                    .Where(s => s.Id == currentUser.Id)
                    .ToList();

                // 3. Extract unique Genres from YOUR segments
                // We use Distinct() so if you have 5 "Rock" segments, "Rock" only appears once.
                var myGenres = mySegments
                    .Where(s => !string.IsNullOrEmpty(s.Genre))
                    .Select(s => s.Genre)
                    .Distinct()
                    .ToList();

                // 4. Bind the data to the UI ListBoxes
                SegmentsListBox.ItemsSource = mySegments;
                GenresListBox.ItemsSource = myGenres;

                // 5. Load instruments (if you have GetInstruments)
                var allInstruments = await apiService.GetInstruments();
                if (allInstruments != null)
                {
                    InstrumentsListBox.ItemsSource = allInstruments
                        .Where(i => i.Id == currentUser.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Musician Area: {ex.Message}");
            }
        }
    }
}
