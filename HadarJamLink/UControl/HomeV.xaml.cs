using ClientSide;
using HadarJamLink;
using JamLinkComputers;
using JamLinkComputers.Pages;
using Microsoft.VisualBasic.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
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
using System.Windows.Media.Effects;
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
        Person loggedInUser;
        public HomeV(Person p)
        {
            InitializeComponent();
            this.loggedInUser = p;
            LoadDynamicDashboard();
            //LoadCards();
        }

        private async void LoadDynamicDashboard()
        {
            try
            {
                ActivityFeedPanel.Children.Clear();
                List<object> mixedFeed = new List<object>();
                Random rnd = new Random();

                // 1. Fetch Data from API
                var segmentsList = await apiService.GetMusicalSegments();
                var peopleList = await apiService.GetPeople();
                var groupsList = await apiService.GetGroupMembers();
                var appsList = await apiService.GetProducerApps();

                // 2. Build Activity Feed (Limit 5 per category)
                mixedFeed.AddRange(segmentsList.OrderByDescending(s => s.Id).Take(5));
                mixedFeed.AddRange(peopleList.Take(5));
                mixedFeed.AddRange(groupsList.Take(5));
                mixedFeed.AddRange(appsList.Take(5));

                // Randomize feed and use the FeedItemV UserControl
                var finalFeed = mixedFeed.OrderBy(x => rnd.Next()).ToList();
                foreach (var item in finalFeed)
                {
                    // Using your FeedItemV UserControl instead of the old manual method
                    ActivityFeedPanel.Children.Add(new FeedItemV(item));
                }

                // 3. Calculate Specific Stats
                // Note: Using the specific property paths from your provided code
                int userSegments = segmentsList.Count(s => s.Musician.Id == loggedInUser.Id);
                int totalGroupsJoined = groupsList.Count;
                int userProjectsCount = groupsList.Count(g => g.Id == loggedInUser.Id);
                int totalApps = appsList.Count;

                StatsLabel.Text = $"{loggedInUser.Username}'s Dashboard";

                // 4. Draw the Rectangular Graph and Legend
                DrawAllStatsGraph(userSegments, totalGroupsJoined, userProjectsCount, totalApps);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sync Error: " + ex.Message);
            }
        }

        private void DrawAllStatsGraph(int segmentCount, int allGroupsCount, int myGroupsCount, int appCount)
        {
            StatsCanvas.Children.Clear();

            // Theme Brushes
            var purpleBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3C205E"));
            var goldBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD55F"));
            var navyBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B0E21"));

            // Draw Y-Axis Numbers and Grid Lines
            for (int i = 0; i <= 15; i += 5)
            {
                double yPos = 200 - (i * 12);
                StatsCanvas.Children.Add(new TextBlock { Text = i.ToString(), Foreground = Brushes.DimGray, FontSize = 10, Margin = new Thickness(-20, yPos - 7, 0, 0) });
                StatsCanvas.Children.Add(new Line { X1 = 0, Y1 = yPos, X2 = 320, Y2 = yPos, Stroke = new SolidColorBrush(Color.FromArgb(30, 128, 128, 128)), StrokeThickness = 1 });
            }

            // Draw the 4 Rectangular Bars
            CreateBar(0, segmentCount, purpleBrush, "Tracks", null);
            CreateBar(1, myGroupsCount, purpleBrush, "My Projects", Brushes.MediumPurple);
            CreateBar(2, allGroupsCount, goldBrush, "Global Grps", null);
            CreateBar(3, appCount, navyBrush, "Tools", Brushes.White);

            // 5. Build Legend
            var legendData = new List<LegendItem>
            {
                new LegendItem { Name = "Personal", Color = purpleBrush },
                new LegendItem { Name = "Community", Color = goldBrush },
                new LegendItem { Name = "Apps", Color = Brushes.White }
            };
            CreateLegend(legendData);
        }

        private void CreateBar(int index, int count, Brush color, string label, Brush borderBrush)
        {
            double barHeight = count * 12;
            if (barHeight > 190) barHeight = 190;
            if (barHeight < 5) barHeight = 5;

            Border bar = new Border
            {
                Width = 35,
                Height = barHeight,
                Background = color,
                BorderBrush = borderBrush ?? Brushes.Transparent,
                BorderThickness = borderBrush != null ? new Thickness(1.5) : new Thickness(0),
                CornerRadius = new CornerRadius(3, 3, 0, 0),
                Margin = new Thickness(20 + (index * 75), 200 - barHeight, 0, 0)
            };

            bar.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = borderBrush != null ? Colors.White : ((SolidColorBrush)color).Color,
                BlurRadius = 8,
                ShadowDepth = 0,
                Opacity = 0.5
            };

            TextBlock textLabel = new TextBlock
            {
                Text = label,
                Foreground = Brushes.LightGray,
                FontSize = 9,
                Width = 70,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(2 + (index * 75), 205, 0, 0)
            };

            StatsCanvas.Children.Add(bar);
            StatsCanvas.Children.Add(textLabel);
        }

        private void CreateLegend(List<LegendItem> items)
        {
            StackPanel legend = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 215, 0, 0) };
            foreach (var item in items)
            {
                StackPanel entry = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 15, 0) };
                entry.Children.Add(new Rectangle { Width = 10, Height = 10, Fill = item.Color });
                entry.Children.Add(new TextBlock { Text = item.Name, Foreground = Brushes.Gray, Margin = new Thickness(5, 0, 0, 0), FontSize = 10 });
                legend.Children.Add(entry);
            }
            StatsCanvas.Children.Add(legend);
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Ready to collaborate, {loggedInUser.Username}?");
        }

        private void ExploreButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. השגת ה-MainWindow בצורה מפורשת (Casting)
                var mainWindow = Window.GetWindow(this) as HadarJamLink.MainWindow;

                if (mainWindow != null)
                {
                    // 2. שימוש ב-MainFrame (השם מה-XAML שלך) ובפעולת Navigate
                    mainWindow.MainFrame.Navigate(new ExploreV(this.loggedInUser));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Navigation Error: " + ex.Message);
            }
        }


        //private void CreateLegend(List<LegendItem> items)
        //{
        //    StackPanel legend = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 210, 0, 0) };
        //    foreach (var item in items)
        //    {
        //        StackPanel entry = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 15, 0) };
        //        entry.Children.Add(new Rectangle { Width = 10, Height = 10, Fill = item.Color });
        //        entry.Children.Add(new TextBlock { Text = item.Name, Foreground = Brushes.Gray, Margin = new Thickness(5, 0, 0, 0), FontSize = 10 });
        //        legend.Children.Add(entry);
        //    }
        //    StatsCanvas.Children.Add(legend);
        //}



        //private void DrawDataLine(int count, Brush color, int offset)
        //{
        //    // Define the progression of the line (X, Y coordinates)
        //    // X goes from 0 to 300; Y is determined by your database count
        //    double peakY = 200 - (count * 12);
        //    if (peakY < 20) peakY = 20; // Keep it within view

        //    Polyline graphLine = new Polyline
        //    {
        //        Stroke = color,
        //        StrokeThickness = 3,
        //        StrokeLineJoin = PenLineJoin.Round, // Makes the corners smooth
        //        Points = new PointCollection
        //{
        //    new Point(0, 190),            // Start (Baseline)
        //    new Point(50, 170),           // Step 1
        //    new Point(150 + offset, peakY),// Step 2 (Your actual DB Stat)
        //    new Point(250, 160),          // Step 3
        //    new Point(350, 185)           // End
        //}
        //    };

        //    // Add a glow effect to make it pop against the navy background
        //    graphLine.Effect = new System.Windows.Media.Effects.DropShadowEffect
        //    {
        //        Color = ((SolidColorBrush)color).Color,
        //        BlurRadius = 15,
        //        ShadowDepth = 0,
        //        Opacity = 0.7
        //    };

        //    StatsCanvas.Children.Add(graphLine);
        //}



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

        private void AIAssistantButton_Click(object sender, RoutedEventArgs e)
        {
            AIChatView chatWindow = new AIChatView();
           
            // If you want to display the chat view, navigate to it or show it in a frame/window.
            // Example: Navigate in MainFrame if available
            var mainWindow = Window.GetWindow(this) as HadarJamLink.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(chatWindow);
            }
            // Otherwise, you may need to host it in a new Window if that's your intent:
            // var window = new Window { Content = chatWindow };
            // window.Show();
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

    // 1. Helper class to avoid Tuple errors entirely
    public class LegendItem
    {
        public string Name { get; set; }
        public Brush Color { get; set; }
    }
}
