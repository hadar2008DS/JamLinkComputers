using ClientSide;
using Microsoft.VisualBasic.ApplicationServices;
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
        private List<MusicianInstruments> myRelations = new();


        // Add this field to your class to represent the BPM slider control
        private System.Windows.Controls.Slider BPMSlider;

        // Add this field to your class to represent the GenreFilter ComboBox
        private System.Windows.Controls.ComboBox GenreFilter;

        public MusicianV(Person loggedInUser)
        {
            InitializeComponent();
            this.currentUser = loggedInUser;
            if (currentUser != null)
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

            myRelations = relations
                .Where(r => r.Musician.Id == currentUser.Id)
                .ToList();

            InstrumentsListBox.ItemsSource = myRelations;
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
                    Musician = null,  // לא שולחים אובייקט מלא
                    Instruments = null
                };

                // יוצרים אובייקטים מינימליים עם Id בלבד
                relation.Musician = new Musician { Id = currentUser.Id };
                relation.Instruments = new Instruments { Id = instrument.Id };

                int result = await apiService.InsertMusicianInstrument(relation);

                if (result > 0)
                    await LoadInstruments();
            }
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
            // לוודא שהעריכה מה-DataGrid נשמרה באובייקט
            SegmentsGrid.CommitEdit();
            SegmentsGrid.CommitEdit();

            if (sender is System.Windows.Controls.Button btn &&
                btn.DataContext is MusicalSegments segment)
            {
                // שולחים רק Id – לא אובייקט מלא
                segment.Musician.Id = currentUser.Id;
                segment.Musician = null;   // חשוב מאוד!

                int result = await apiService.UpdateMusicalSegment(segment);

                if (result > 0)
                    System.Windows.MessageBox.Show("Segment updated successfully!");
                else
                    System.Windows.MessageBox.Show("Update failed.");
            }
        }

        //private async void AddInstrument_Click(object sender, RoutedEventArgs e)
        //{
        //    if (AllInstrumentsCombo.SelectedItem is Instruments instrument)
        //    {
        //        var relation = new MusicianInstruments
        //        {
        //            Musician = new Musician { Id = currentUser.Id },
        //            Instruments = new Instruments { Id = instrument.Id }
        //        };

        //        int result = await apiService.InsertMusicianInstrument(relation);

        //        if (result > 0)
        //            await LoadInstruments();
        //    }
        //}

        private async void DeleteInstrument_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn &&
                btn.DataContext is MusicianInstruments relation)
            {
                int result = await apiService.DeleteMusicianInstrument(relation.Id);

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

            var newSegment = new MusicalSegments
            {
                SegmentName = "New Segment",
                Genre = newGenre,
                Lengthinseconds = 60,
                Bpm = 120,

                // Set the Musician property with only the Id set
                Musician = new Musician { Id = currentUser.Id }
            };

            int result = await apiService.InsertMusicalSegment(newSegment);

            if (result > 0)
            {
                await LoadSegments();
                await LoadGenres();
                NewGenreTextBox.Clear();
            }
        }

    }
}
