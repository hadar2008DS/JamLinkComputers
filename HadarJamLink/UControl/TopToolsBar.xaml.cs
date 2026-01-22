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
    /// Interaction logic for TopToolsBar.xaml
    /// </summary>
    public partial class TopToolsBar : UserControl
    {
        public event Action<string> ToolSelected;
        public TopToolsBar()
        {
            InitializeComponent();
        }
        private void Tuner_Click(object sender, RoutedEventArgs e)
        {
            ToolSelected?.Invoke("Tuner");
        }

        private void Scales_Click(object sender, RoutedEventArgs e)
        {
            ToolSelected?.Invoke("Scales");
        }

        private void Chords_Click(object sender, RoutedEventArgs e)
        {
            ToolSelected?.Invoke("Chords");
        }

        private void Metronome_Click(object sender, RoutedEventArgs e)
        {
            ToolSelected?.Invoke("Metronome");
        }
    }
}
