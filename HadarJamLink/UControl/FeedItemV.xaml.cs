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
    /// Interaction logic for FeedItemV.xaml
    /// </summary>
    public partial class FeedItemV : UserControl
    {
        public FeedItemV(object data)
        {
            InitializeComponent();
            PopulateCard(data);
        }
        private void PopulateCard(object data)
        {
            if (data is MusicalSegments s)
            {
                TitleTxt.Text = s.SegmentName;
                SubTxt.Text = $"{s.Bpm} BPM | {s.Genre}";
                IconTxt.Text = "🎵";
            }
            else if (data is Person p)
            {
                TitleTxt.Text = p.Username;
                SubTxt.Text = "Member Profile";
                IconTxt.Text = "👤";
            }
            else if (data is GroupMembers g)
            {
                TitleTxt.Text = "Group Joined";
                SubTxt.Text = $"Project ID: {g.Group.Id}";
                IconTxt.Text = "👥";
            }
            else if (data is ProducerApps pa)
            {
                TitleTxt.Text = "App Linked";
                SubTxt.Text = $"Tool ID: {pa.Apps.AppName}";
                IconTxt.Text = "🎹";
            }
        }
    }
}

