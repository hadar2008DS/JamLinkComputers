using ClientSide;
using Microsoft.VisualBasic.ApplicationServices;
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
using HadarJamLink;


namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for PublicProfileV.xaml
    /// </summary>
    public partial class PublicProfileV : UserControl
    {
        private ApiService apiService = new ApiService();
        private Person targetUser;
        private Person loggedInUser;
        public PublicProfileV(Person userToShow, Person loggedInUser)
        {
            InitializeComponent();
            targetUser = userToShow;
            this.loggedInUser = loggedInUser;
            LoadUserData();
        }

        private async void LoadUserData()
        {
            try
            {
                // 1. הגדרת שם המשתמש בכותרת
                UserNameTitle.Text = targetUser.Username;

                // 2. שליפת סגמנטים מוזיקליים של המשתמש הזה
                var allSegments = await apiService.GetMusicalSegments();
                var userSegments = allSegments.Where(s => s.Musician != null && s.Musician.Id == targetUser.Id).ToList();

                SegmentsItemsControl.ItemsSource = userSegments;

                // 3. שליפת ז'אנרים ייחודיים מתוך הסגמנטים שלו
                var userGenres = userSegments.Select(s => s.Genre).Distinct().ToList();
                GenresItemsControl.ItemsSource = userGenres;

                // 4. שליפת כלי נגינה (בהנחה שיש לך פונקציה כזו ב-API)
                // אם הכלים נמצאים בתוך אובייקט המשתמש, אפשר להשתמש בהם ישירות
                // כאן אני שולף את הכלים שמוצמדים לסגמנטים שלו כדוגמה:
                var userInstruments = userSegments
                    .Where(s => s.Instruments != null)
                    .Select(s => s.Instruments)
                    .GroupBy(i => i.Id) // מניעת כפילויות של אותו כלי
                    .Select(g => g.First())
                    .ToList();

                InstrumentsItemsControl.ItemsSource = userInstruments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile: " + ex.Message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // תיקון הניווט חזרה לדף ה-Explore
            var mainWindow = Window.GetWindow(this) as HadarJamLink.MainWindow;

            if (mainWindow != null)
            {
                // שימוש ב-Navigate במקום ב-Children.Add
                mainWindow.MainFrame.Navigate(new ExploreV(this.loggedInUser));
            }
        }
    }
}
