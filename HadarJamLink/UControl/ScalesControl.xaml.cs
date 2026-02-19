using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ScalesControl.xaml
    /// </summary>
    public partial class ScalesControl : UserControl
    {

        //insperation:
        //https://www.all-guitar-chords.com/scales
        public ScalesControl()
        {
            InitializeComponent();
            // This is the "bridge" between your XAML and your C# logic
            this.DataContext = new MainViewModel();
        }
    }


    public static class MusicHelper
    {
        // The "Chromatic Scale": All possible notes. 
        // We use this as a reference index (C=0, C#=1, etc.)
        public static readonly string[] Notes = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        // Dictionary of scale "formulas." 
        // The numbers represent how many half-steps to move away from the Root.
        // E.g., Major (0,2,4...) means: Root, +2 frets, +4 frets, etc.
        public static readonly Dictionary<string, int[]> ScalePatterns = new Dictionary<string, int[]>
    {
        { "Major", new[] { 0, 2, 4, 5, 7, 9, 11 } },
        { "Natural Minor", new[] { 0, 2, 3, 5, 7, 8, 10 } },
        // ... other patterns follow the same logic
    };

        public static List<string> CalculateScale(string root, string patternName)
        {
            // 1. Find where the chosen Root note sits in our Chromatic array (0-11)
            int rootIndex = Array.IndexOf(Notes, root);

            // 2. Grab the interval formula (e.g., { 0, 2, 4... }) for the selected scale
            var pattern = ScalePatterns[patternName];

            // 3. Math time: (Root Index + Interval) % 12. 
            // We use % 12 (Modulo) so if we go past "B", we wrap back around to "C".
            return pattern.Select(i => Notes[(rootIndex + i) % 12]).ToList();
        }
    }

    // RelayCommand implementation added to fix a bug
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute; // The logic to run
        private readonly Predicate<object> canExecute; // Optional: condition to check if button is enabled

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        // Tells WPF if the button is allowed to be clicked
        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        // This is triggered when the user clicks the button
        public void Execute(object parameter) => execute(parameter);

        // Forces the UI to re-check if the button should be enabled/disabled
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        // Provide the lists for the UI to display in the Buttons/ComboBoxes
        public string[] AllNotes => MusicHelper.Notes;
        public IEnumerable<string> AllScales => MusicHelper.ScalePatterns.Keys;

        // Internal storage for user selections
        private string selectedRoot = "C";
        private string selectedScale = "Major";

        // Formats a title like "D# Dorian" for the screen
        public string SelectedScaleTitle => $"{selectedRoot} {selectedScale}";

        // The logic that actually generates the string shown in the UI Result area
        public string DisplayResult => string.Join("  ", MusicHelper.CalculateScale(selectedRoot, selectedScale));

        // This Command is called when you click a Note button (C, C#, D...)
        public ICommand SelectRootCommand => new RelayCommand(param => {
            selectedRoot = param.ToString(); // Set the new Root
            RefreshUI(); // Tell the UI to update the text
        });

        // This Command is called when you click a Scale button (Major, Minor...)
        public ICommand SelectScaleCommand => new RelayCommand(param => {
            selectedScale = param.ToString(); // Set the new Scale type
            RefreshUI(); // Tell the UI to update the text
        });

        // Crucial: This tells WPF; The data has changed,please redraw the screen
        private void RefreshUI()
        {
            OnPropertyChanged(nameof(DisplayResult));
            OnPropertyChanged(nameof(SelectedScaleTitle));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
