using ClientSide;
using JamLinkComputers.UControl;
using Model;
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        private MetronomeControl metronomeInstance;
        private TunerControl TunerInstance;
        private ChordsControl ChordsInstance;
        private ScalesControl ScalesInstance;
        private ExploreV ExploreInstance;

        private MediaPlayer backgroundPlayer = new MediaPlayer();
        private bool isMuted = false;

        public UserHomePage(Person user)
        {
            InitializeComponent();
            InitializeBackgroundAudio();

            currentUser = user;

            Loaded += UserHomePage_Loaded;
        }

        private async void UserHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            // מציאת ה־Sidebar מתוך ה־XAML
            SideBarBTNInstance = FindName("SideBarBTN") as SideBarBTN;

            if (SideBarBTNInstance != null)
                SideBarBTNInstance.MenuClicked += SideBar_MenuClicked;

            await LoadUserRole();

            // ברירת מחדל
            LoadView(new HomeV(currentUser));
        }

        public void NavigateToHome()
        {
            // משתמש בפונקציית ה-LoadView שכבר קיימת אצלך
            LoadView(new HomeV(currentUser));
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

        public void SideBar_MenuClicked(string menu)
        {
            switch (menu)
            {
                case "Home":
                    LoadView(new HomeV(currentUser));
                    break;

                case "Profile":
                    LoadView(new ProfileV(currentUser));
                    break;

                case "Groups":
                    LoadView(new GroupsV(currentUser));
                    break;

                case "Musician":
                    LoadView(new MusicianV(currentUser));
                    break;

                case "Producer":
                    LoadView(new ProducerV(currentUser));
                    break;
                case "Explore":
                    MainContent.Content = CreateExploreView();
                    break;
            }
        }

        // ================= VIEW LOADER =================

        public void LoadView(UserControl view)
        {
            MainContent.Content = view;
        }
        private void ProducerBtn_Click(object sender, RoutedEventArgs e) { }

        private void MusicianBtn_Click(object sender, RoutedEventArgs e) { }

        private void SideBarBTN_Loaded(object sender, RoutedEventArgs e) { }

        private UIElement CreateTunerView()
        {
            if (TunerInstance == null)
                TunerInstance = new TunerControl();

            return TunerInstance;
        }

        private UIElement CreateScalesView()
        {

            if (ScalesInstance == null)
                ScalesInstance = new ScalesControl();

            return ScalesInstance;
        }

        private UIElement CreateChordsView()
        {
            if (ChordsInstance == null)
                ChordsInstance = new ChordsControl();

            return ChordsInstance;
        }

        private UIElement CreateMetronomeView()
        {
            if (metronomeInstance == null)
                metronomeInstance = new MetronomeControl();

            return metronomeInstance;
        }
        private UIElement CreateExploreView()
        {
            if (ExploreInstance == null)
                ExploreInstance = new ExploreV(currentUser);

            return ExploreInstance;
        }
        private void Tuner_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreateTunerView();
        }

        private void Scales_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreateScalesView();
        }

        private void Chords_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreateChordsView();
        }

        private void Metronome_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreateMetronomeView();
        }

        private void Tips_Click(object sender, RoutedEventArgs e)
        {
            Grid mainGrid = new Grid { Margin = new Thickness(0) }; 
            mainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF112240"));
            mainGrid.Children.Add(new Border { Padding = new Thickness(30) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var titleFont = new FontFamily("Segoe UI Bold");
            var textFont = new FontFamily("Segoe UI Semibold");
            var cyanBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D2FF"));
            var cardBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0A192F"));

            // ================= הטור של המוזיקאים =================
            StackPanel musicianPanel = new StackPanel { Margin = new Thickness(15) };
            musicianPanel.Children.Add(new TextBlock
            {
                Text = "🎸 FOR MUSICIANS",
                FontSize = 22,
                FontFamily = titleFont,
                Foreground = cyanBrush,
                Margin = new Thickness(0, 0, 0, 20)
            });

            string[] musicianTips = {
                    " Use the Metronome to practice complex riffs slowly, then speed up gradually. 🥁",
                    " Record your practice sessions! It is the best way to catch timing and pitch errors. 📱",
                    " Protect your ears. Always use musician earplugs during loud rehearsals. 🎧",
                    " Master your scales. It is the absolute key to fluent and effortless improvisation. 🎹",
                    " Warm up for at least 5-10 minutes before singing or playing to prevent injuries. 🎤"
                };

            foreach (var tip in musicianTips)
            {
                Border card = CreateTipCard(tip, textFont, cardBg);
                musicianPanel.Children.Add(card);
            }
            Grid.SetColumn(musicianPanel, 0);
            mainGrid.Children.Add(musicianPanel);
            // ================= הטור של המפיקים =================
            StackPanel producerPanel = new StackPanel { Margin = new Thickness(15) };
            producerPanel.Children.Add(new TextBlock
            {
                Text = "🎛️ FOR PRODUCERS",
                FontSize = 22,
                FontFamily = titleFont,
                Foreground = cyanBrush,
                Margin = new Thickness(0, 0, 0, 20)
            });

            string[] producerTips = {
                " Trust your ears, not just your eyes. Don't over-rely on visual EQ analyzers. 👁️",
                " Mix at lower volumes to avoid ear fatigue and get a better balance of the levels. 📉",
                " Clear the mud! Use a High-Pass Filter (HPF) on non-bass tracks to free up headroom. 🧹",
                " Organization is key. Color-code your tracks and name them properly before mixing. 🎨",
                " Take regular breaks. 5 minutes away from the monitors can completely refresh your mix perspective. ☕"
            };

            foreach (var tip in producerTips)
            {
                Border card = CreateTipCard(tip, textFont, cardBg);
                producerPanel.Children.Add(card);
            }
            Grid.SetColumn(producerPanel, 1);
            mainGrid.Children.Add(producerPanel);

            MainContent.Content = mainGrid;
        }

        private Border CreateTipCard(string text, FontFamily font, Brush background)
        {
            Border border = new Border
            {
                Background = background, 
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 12),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222244")),
                BorderThickness = new Thickness(1)
            };

            border.Child = new TextBlock
            {
                Text = text,
                FontSize = 15,
                Foreground = Brushes.White,
                FontFamily = font,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };

            return border;
        }

        private void ToggleSideBar_Click(object sender, RoutedEventArgs e)
        {
            if (SideBarColumn.Width.Value > 0)
                SideBarColumn.Width = new GridLength(0); // Close fully
            else
                SideBarColumn.Width = new GridLength(120); // Open
        }



        private void InitializeBackgroundAudio()
        {
            try
            {
                // שימוש ב-Path.Combine המומלץ לבניית נתיב יציב ומדויק
                string audioPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio", "ApetureInstrumentals.wav");

                // בדיקה בדיבאגר (Output) האם הקובץ באמת קיים פיזית בנתיב הריצה
                if (!System.IO.File.Exists(audioPath))
                {
                    System.Diagnostics.Debug.WriteLine(" Audio file missing at: " + audioPath);
                    return;
                }

                backgroundPlayer.Open(new Uri(audioPath, UriKind.Absolute));

                // יצירת לופ אוטומטי - כשהשיר מסתיים, חוזרים להתחלה ומנגנים שוב
                backgroundPlayer.MediaEnded += (s, e) =>
                {
                    backgroundPlayer.Position = TimeSpan.Zero;
                    backgroundPlayer.Play();
                };

                backgroundPlayer.Volume = 0.5; // ווליום של 50%
                backgroundPlayer.Play();       // הפעלה מידית
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(" Audio load error: " + ex.Message);
            }
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (backgroundPlayer == null) return;

            if (!isMuted)
            {
                backgroundPlayer.IsMuted = true; // השתקה מובנית של הנגן
                isMuted = true;

                MuteIcon.Text = "🔇";
                MuteIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8892B0"));
            }
            else
            {
                backgroundPlayer.IsMuted = false; // ביטול השתקה
                isMuted = false;

                MuteIcon.Text = "🔊";
                MuteIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D2FF"));
            }
        }
    }
}
