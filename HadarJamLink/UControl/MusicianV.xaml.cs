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
    /// Interaction logic for MusicianV.xaml
    /// </summary>
    public partial class MusicianV : UserControl
    {
        private Person currentUser;
        public MusicianV()
        {
            InitializeComponent();
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Audio upload will be added later.");
        }
    }
}
