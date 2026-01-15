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
        private Person currentUser;

        public ProfileV(Person user)
        {
            InitializeComponent();
            if (user == null)
            {
                UserNameText.Text = "User not loaded";
                return;
            }
            currentUser = user;
            LoadProfile();
        }
        private void LoadProfile()
        {
            UserNameText.Text = $"Username: {currentUser.Username}";
            StatusText.Text = currentUser.IsActive ? "Status: Active" : "Status: Inactive";
        }


    }
}
