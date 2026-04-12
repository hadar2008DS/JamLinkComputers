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
            if (AllInstrumentsCombo.SelectedItem is Instruments selectedInstrument)
            {
                try
                {
                    var relation = new MusicianInstruments
                    {
                        // אנחנו חייבים למלא את כל שדות החובה שה-API דורש
                        Musician = new Musician
                        {
                            Id = currentUser.Id,
                            Username = currentUser.Username, // שדה חובה לפי השגיאה
                            PassW = currentUser.PassW       // שדה חובה לפי השגיאה
                        },
                        Instruments = new Instruments
                        {
                            Id = selectedInstrument.Id,
                            InstrumentName = selectedInstrument.InstrumentName // שדה חובה לפי השגיאה
                        }
                    };

                    int result = await apiService.InsertMusicianInstrument(relation);

                    if (result > 0)
                    {
                        await LoadInstruments();
                        // אופציונלי: איפוס ה-Combo לאחר הוספה
                        AllInstrumentsCombo.SelectedIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Could not add instrument: {ex.Message}");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an instrument first.");
            }
        }


        private async Task LoadSegments()
        {
            var segments = await apiService.GetMusicalSegments();
            if (segments != null)
            {
                // Update the list that LoadGenres depends on
                allMySegments = segments
                    .Where(s => s.Musician != null && s.Musician.Id == currentUser.Id)
                    .ToList();

                SegmentsGrid.ItemsSource = null;
                SegmentsGrid.ItemsSource = allMySegments;
            }
        }

        private async void UpdateSegment_Click(object sender, RoutedEventArgs e)
        {
            // Ensure DataGrid changes are committed to the bound object
            SegmentsGrid.CommitEdit();

            if (sender is System.Windows.Controls.Button btn &&
                btn.DataContext is MusicalSegments segment)
            {
                try
                {
                    // Attach user info and clear object reference to prevent API circular reference errors
                    segment.Musician = new Musician
                    {
                        Id = currentUser.Id,
                        Username = currentUser.Username,
                        PassW = currentUser.PassW
                    };

                    int result = await apiService.UpdateMusicalSegment(segment);

                    if (result > 0)
                        System.Windows.MessageBox.Show("Segment updated successfully!");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Update failed: {ex.Message}");
                }
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
            // 1. וודאי שהרשימה הגלובלית חזרה מהשרת ומכילה נתונים
            if (allMySegments == null || allMySegments.Count == 0)
            {
                GenresListBox.ItemsSource = null;
                return;
            }

            // 2. שליפת הז'אנרים הייחודיים
            var genres = allMySegments
                .Select(s => s.Genre)
                .Where(g => !string.IsNullOrEmpty(g))
                .Distinct()
                .ToList();

            // 3. התיקון הקריטי: איפוס ה-ItemsSource כדי להכריח את ה-UI להתרענן
            GenresListBox.ItemsSource = null;
            GenresListBox.ItemsSource = genres;
        }

        private async void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            string newGenre = NewGenreTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(newGenre)) return;

            try
            {
                var newSegment = new MusicalSegments
                {
                    SegmentName = "New Composition",
                    Genre = newGenre,
                    Musician = new Musician { Id = currentUser.Id, Username = currentUser.Username, PassW = currentUser.PassW },
                    Instruments = null
                };

                int result = await apiService.InsertMusicalSegment(newSegment);

                if (result > 0)
                {
                    // המתנה של חצי שנייה כדי שה-Database יתעדכן בשרת
                    await Task.Delay(500);

                    // 1. טעינת כל הסגמנטים מהשרת (מעדכן את allMySegments)
                    await LoadSegments();

                    // 2. בניית רשימת הז'אנרים מהסגמנטים שנטענו
                    await LoadGenres();

                    NewGenreTextBox.Clear();
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        }

    }
}
