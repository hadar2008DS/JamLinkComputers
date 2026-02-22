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
    /// Interaction logic for ProfileV.xaml
    /// </summary>
    public partial class ProfileV : UserControl
    {
        ApiService apiService = new ApiService();

        private Person loggedInUser;

        private bool isMusician = false;
        private bool isProducer = false;


        public ProfileV(Person user)
        {
            InitializeComponent();
            if (user == null)
            {
                GreetingText.Text = "User not loaded";
                return;
            }

            loggedInUser = user;

            LoadProfile();
        }


        private async void LoadProfile()
        {
            try
            {
                GreetingText.Text = $"{GetGreeting()}, {loggedInUser.Username}";
                ActiveToggle.IsChecked = loggedInUser.IsActive;

                // 1. בדיקה אם הוא מוזיקאי
                MusicianList mList = await apiService.GetMusicians();
                if (mList != null)
                {
                    var allInstruments = await apiService.GetInstruments(); // טבלת Instruments
                    var musicianInstrumentsLink = await apiService.GetMusicianInstruments(); // טבלת MusicianInstruments (המקשרת)

                    Musician m = mList.Find(x => x.Id == loggedInUser.Id);

                    if (m != null)
                    {
                        isMusician = true;
                        MusicianCard.Visibility = Visibility.Visible;

                        // 2. מציאת כל ה-IDs של הכלים ששייכים למוזיקאי הזה מהטבלה המקשרת
                        // לפי הדיאגרמה השדה הוא Id_musician
                        var myInstrumentIds = musicianInstrumentsLink
                            .Where(mi => mi.Musician.Id == m.Id)
                            .Select(mi => mi.Instruments.Id)
                            .ToList();

                        // 3. שליפת השמות של הכלים מטבלת Instruments
                        var myInstrumentNames = allInstruments
                            .Where(i => myInstrumentIds.Contains(i.Id))
                            .Select(i => i.InstrumentName)
                            .ToList();

                        // 4. הצגה בטקסט (למשל: "Guitar, Piano")
                        if (myInstrumentNames.Any())
                        {
                            InstrumentText.Text = "Instruments: " + string.Join(", ", myInstrumentNames);
                        }
                        else
                        {
                            InstrumentText.Text = "No instruments defined";
                        }
                    }

                    // 2. בדיקה אם הוא מפיק
                    PreducerList prList = await apiService.GetProducers();
                    if (prList != null)
                    {
                        Producer p = prList.Find(x => x.Id == loggedInUser.Id);
                        if (p != null)
                        {
                            isProducer = true;
                            ProducerCard.Visibility = Visibility.Visible; // מציג את הכרטיס

                            // חיבור רשימת האפליקציות (וודאי שבמודל Producer יש רשימה בשם Apps)
                            // AppsList.ItemsSource = p.Apps;
                        }
                    }

                    RoleText.Text = GetRoleText();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile: " + ex.Message);
            }
        }

        private async void ActiveToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (loggedInUser == null) return;

            bool isActive = ActiveToggle.IsChecked ?? false;
            loggedInUser.IsActive = isActive;

            // 1. Immediate UI Feedback (Before the API call)
            UpdateStatusUI(isActive);

            try
            {
                int result = await apiService.UpdatePerson(loggedInUser);

                if (result > 0)
                {
                    ActiveToggle.ToolTip = "Status synchronized with server!";
                }
                else
                {
                    // Revert UI if server update failed
                    MessageBox.Show("Server update failed.");
                    ActiveToggle.IsChecked = !isActive;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                ActiveToggle.IsChecked = !isActive; // Revert on error
            }
        }

        // Helper method to handle the "Look" of the status
        private void UpdateStatusUI(bool isActive)
        {
            if (isActive)
            {
                StatusLabel.Text = "Active";
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(253, 203, 88)); // Your Gold (#FDCB58)
            }
            else
            {
                StatusLabel.Text = "Inactive";
                StatusLabel.Foreground = Brushes.Gray;
            }
        }

        // 2. שינוי שם משתמש וסיסמה
        private async void UpdateDetails_Click(object sender, RoutedEventArgs e)
        {
            // בדיקה אם יש טקסט בתיבות
            if (string.IsNullOrWhiteSpace(NewUsernameInput.Text) && string.IsNullOrWhiteSpace(NewPasswordInput.Password))
            {
                MessageBox.Show("Please enter new details.");
                return;
            }

            // עדכון הערכים בתוך האובייקט שמחזיק את המשתמש המחובר
            if (!string.IsNullOrWhiteSpace(NewUsernameInput.Text))
                loggedInUser.Username = NewUsernameInput.Text;

            if (!string.IsNullOrWhiteSpace(NewPasswordInput.Password))
                loggedInUser.PassW = NewPasswordInput.Password;

            try
            {
                var result = await apiService.UpdatePerson(loggedInUser);

                if (result > 0) // בדיקה אם המספר מעיד על הצלחה
                {
                    MessageBox.Show("Details updated successfully!");
                    GreetingText.Text = $"{GetGreeting()}, {loggedInUser.Username}";

                    NewUsernameInput.Clear();
                    NewPasswordInput.Clear();
                }
                
            }
            catch (Exception ex)
            {
                    MessageBox.Show("Update failed. Please try again."); 
            }
        }

        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;

            if (hour < 12)
                return "Good Morning";
            if (hour < 18)
                return "Good Afternoon";

            return "Good Evening";
        }

        private string GetRoleText()
        {
            if (isMusician && isProducer)
                return "Musician & Producer";

            if (isMusician)
                return "Musician";

            if (isProducer)
                return "Producer";

            return "";
        }
    }
}
