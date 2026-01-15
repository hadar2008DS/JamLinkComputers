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
    /// Interaction logic for GroupsV.xaml
    /// </summary>
    public partial class GroupsV : UserControl
    {
        private Person currentUser;
        public GroupsV()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            // Dummy data (later comes from DB)
            GroupsList.Items.Add("Jazz Project");
            GroupsList.Items.Add("Rock Band");
            GroupsList.Items.Add("Electronic Collab");
        }
    }
}
