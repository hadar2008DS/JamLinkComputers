using ClientSide;
using Microsoft.VisualBasic.Logging;
using Model;
using NAudio.SoundFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for MusicianV.xaml
    /// </summary>
    public partial class MusicianV : System.Windows.Controls.UserControl
    {
        private Person currentUser;
        private ApiService apiService = new ApiService();

        private List<MusicalSegments> allMySegments = new();

        // Add this field to your class to represent the BPM slider control
        private System.Windows.Controls.Slider BPMSlider;

        // Add this field to your class to represent the GenreFilter ComboBox
        private System.Windows.Controls.ComboBox GenreFilter;

        public MusicianV(Person loggedInUser)
        {
            InitializeComponent();
            this.currentUser = loggedInUser;
            //BPMSlider.ValueChanged += FilterChanged;
            //GenreFilter.SelectionChanged += FilterChanged;
            LoadMusicianData();
        }

        private async void LoadMusicianData()
        {
            try
            {
                await LoadSegments();
                await LoadGenres();
                await LoadInstruments();
                await LoadAllInstrumentsForCombo();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private async Task LoadInstruments()
        {
            var relations = await apiService.GetMusicianInstruments();

            var myInstrumentIds = relations
                .Where(r => r.Musician.Id == currentUser.Id)
                .Select(r => r.Instruments.Id)
                .ToList();

            var instruments = await apiService.GetInstruments();

            InstrumentsListBox.ItemsSource = instruments
                .Where(i => myInstrumentIds.Contains(i.Id))
                .ToList();
        }

        private async Task LoadSegments()
        {
            var segments = await apiService.GetMusicalSegments();

            allMySegments = segments
                .Where(s => s.Musician.Id == currentUser.Id)
                .ToList();

            SegmentsGrid.ItemsSource = allMySegments;
        }

        private async void UpdateSegment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Forms.Button btn &&
                btn.DataContext is MusicalSegments segment)
            {
                var result = await apiService.UpdateMusicalSegment(segment);

                if (result > 0)
                    System.Windows.MessageBox.Show("Segment updated!");
                else
                    System.Windows.MessageBox.Show("Update failed.");
            }
        }
        private async Task LoadAllInstrumentsForCombo()
        {
            var allInstruments = await apiService.GetInstruments();
            AllInstrumentsCombo.ItemsSource = allInstruments;
        }

        private async void AddInstrument_Click(object sender, RoutedEventArgs e)
        {
            if (AllInstrumentsCombo.SelectedItem is Instruments instrument)
            {
                var relation = new MusicianInstruments
                {
                    Musician = currentUser as Musician,
                    Instruments = instrument
                };

                int result = await apiService.InsertMusicianInstrument(relation);

                if (result > 0)
                    await LoadInstruments();
            }
        }

        private async void DeleteInstrument_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Forms.Button btn &&
                btn.DataContext is Instruments instrument)
            {
                // Use only the musician instrument ID as per the method signature
                int result = await apiService.DeleteMusicianInstrument(instrument.Id);

                if (result > 0)
                    await LoadInstruments();
            }
        }

        private async Task LoadGenres()
        {
            var genres = allMySegments
                .Select(s => s.Genre)
                .Where(g => !string.IsNullOrEmpty(g))
                .Distinct()
                .ToList();

            GenresListBox.ItemsSource = genres;
        }

        private async void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            string newGenre = NewGenreTextBox.Text;

            if (string.IsNullOrWhiteSpace(newGenre))
                return;

            allMySegments.Add(new MusicalSegments
            {
                Musician = currentUser as Musician,
                Genre = newGenre
                // Add other required properties if needed
            });

            await LoadGenres();
            NewGenreTextBox.Clear();
        }

    }
}
