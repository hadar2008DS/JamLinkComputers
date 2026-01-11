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

namespace JamLinkComputers.UserControls
{
    /// <summary>
    /// Interaction logic for SideBarButton.xaml
    /// </summary>
    public partial class SideBarButton : System.Windows.Controls.UserControl
    {
        public event Action<string> MenuClicked;

        public SideBarButton()
        {
            InitializeComponent();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        => MenuClicked?.Invoke("Home");

        private void Profile_Click(object sender, RoutedEventArgs e)
            => MenuClicked?.Invoke("Profile");

        private void Groups_Click(object sender, RoutedEventArgs e)
            => MenuClicked?.Invoke("Groups");

        private void MusicianBtn_Click(object sender, RoutedEventArgs e)
            => MenuClicked?.Invoke("Musician");

        private void ProducerBtn_Click(object sender, RoutedEventArgs e)
            => MenuClicked?.Invoke("Producer");
    }
}
