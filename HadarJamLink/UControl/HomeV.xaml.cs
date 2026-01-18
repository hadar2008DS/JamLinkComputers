using ClientSide;
using HadarJamLink;
using JamLinkComputers;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for HomeV.xaml
    /// </summary>
    public partial class HomeV : UserControl
    {
        ApiService apiService = new ApiService();
        private Person CurrentUser => (Person)Application.Current.Properties["CurrentUser"];
        public HomeV()
        {
            InitializeComponent();
            //LoadCards();
        }

        //private async void LoadCards()
        //{
        //    // ניקוי ה-WrapPanel לפני הטעינה (למקרה של רענון)
        //    CardViews.Children.Clear();

        //    // 1. הוספת קבוצות (Groups)
        //    DataTable groupsTable = apiService.GetGroups();
        //    foreach (DataRow row in groupsTable.Rows)
        //    {
        //        var card = new CardViews();
        //        card.TxtTitle.Text = row["GroupName"].ToString();
        //        card.TxtInfo.Text = "Created: " + row["CreationDate"].ToString();
        //        card.TxtType.Text = "Group";

        //        // הוספה ל-WrapPanel שכתבת ב-XAML
        //        CardViews.Children.Add(card);
        //    }

        //    // 2. הוספת קטעי מוזיקה (Musical Segments)
        //    DataTable segmentsTable = apiService.GetMusicalSegments();
        //    foreach (DataRow row in segmentsTable.Rows)
        //    {
        //        var card = new CardViews();
        //        card.TxtTitle.Text = row["SegmentName"].ToString();
        //        card.TxtInfo.Text = row["Genre"].ToString() + " | " + row["BPM"].ToString() + " BPM";
        //        card.TxtType.Text = "Music";

        //        CardViews.Children.Add(card);
        //    }
        //}
        private void LoadHomeFeed()
        {
            //LoadMusicSegments();
            //LoadGroups();
        }
        //private async void LoadMusicSegments()
        //{
        //    AppsList aList = await apiService.GetApps();
        //    foreach (var a in aList) {
        //        TextBlock tbApp = new TextBlock
        //        {
        //            Text = " " + a.AppName,
        //            Foreground = Brushes.White,
        //            Margin = new Thickness(0, 5, 0, 0)
        //        };
        //        AppsContainer.Children.Add(tbApp);
        //    }

        //    SegmentsContainer.Children.Clear();

        //    MusicalSegmentsList allSegments = await apiService.GetMusicalSegments();
        //    foreach (var segment in allSegments)
        //    {
        //        TextBlock tb = new TextBlock
        //        {
        //            Text = " " + segment.SegmentName,
        //            Foreground = Brushes.White,
        //            Margin = new Thickness(0, 5, 0, 0)
        //        };

        //        SegmentsContainer.Children.Add(tb);
        //    }
        //}
        //private void LoadGroups()
        //{
        //    GroupsContainer.Children.Clear();

        //    // Example: groups that the user is part of
        //    int currentUserId = CurrentUser.Id;

        //    // Assuming you have a collection of groups, e.g., Model.GroupList AllGroups
        //    var userGroups = Model.Group
        //        .Where(g => g.Members.Any(m => m.UserId == currentUserId))
        //        .OrderByDescending(g => g.CreatedAt)
        //        .Take(5);

        //    foreach (var group in userGroups)
        //    {
        //        TextBlock tb = new TextBlock
        //        {
        //            Text = " " + group.GroupName,
        //            Foreground = Brushes.White,
        //            Margin = new Thickness(0, 5, 0, 0)
        //        };

        //        GroupsContainer.Children.Add(tb);
        //    }
        //}

    }
}
