using ClientSide;
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
    /// Interaction logic for CardViews.xaml
    /// </summary>
    public partial class CardViews : UserControl
    {
        ApiService apiService = new ApiService();
        public CardViews()
        {
            InitializeComponent();
        }
        // הוסיפי async כדי שתוכלי להשתמש ב-await
        private async void LoadCards()
        {
            // ניקוי ה-WrapPanel לפני הטעינה
            //CardsContainer.Children.Clear();

            try
            {
                // 1. טעינת קבוצות (Groups)
                // משתמשים ב-await כי הפונקציה מחזירה Task
                var groupsResponse = await apiService.GetGroups();

                // נניח ש-GroupList מכיל רשימה של אובייקטים בשם Groups
                foreach (var group in groupsResponse)
                {
                    //var card = new CardViews();
                    //card.TxtTitle.Text = group.GroupName; // שימוש ישיר במודל במקום DataTable
                    //card.TxtInfo.Text = "Created: " + group.CreationDate.Value.ToString("dd/MM/yyyy");
                    //card.TxtType.Text = "Group";

                    //CardsContainer.Add(card);
                }

                // 2. טעינת קטעי מוזיקה (Musical Segments)
                var segmentsResponse = await apiService.GetMusicalSegments();

                foreach (var segment in segmentsResponse)
                {
                    //var card = new CardViews();
                    //card.TxtTitle.Text = segment.SegmentName;
                    //card.TxtInfo.Text = $"{segment.Genre} | {segment.Bpm} BPM";
                    //card.TxtType.Text = "Music";

                 //CardsContainer.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
    }
}
