using CommunityToolkit.Mvvm.ComponentModel; // Provides MVVM attributes and base classes (not directly used here but commonly available).
using CommunityToolkit.Mvvm.Input; // Provides RelayCommand and input helpers (kept for possible command bindings).
using Microsoft.Web.WebView2.Core; // Types for WebView2 navigation events and control integration.
using System; 
using System.Collections.Generic; 
using System.ComponentModel; 
using System.Linq; 
using System.Runtime.CompilerServices; 
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
    /// Interaction logic for ChordsControl.xaml
    /// </summary>
    public partial class ChordsControl : UserControl
    {
        // A readonly list of the exact URLs the control will allow the embedded WebView2 to navigate to.
        // Keeping this list readonly prevents accidental reassignment at runtime.
        private readonly List<string> _allowedUrls = new List<string>
        {
            "https://muted.io/piano-chords/",   // Allowed: Piano chord reference page
            "https://muted.io/guitar-chords/",  // Allowed: Guitar chord reference page
            "https://muted.io/ukulele-chords/"  // Allowed: Ukulele chord reference page
        };

        
        public ChordsControl()
        {
            InitializeComponent(); 

            this.DataContext = this; 

            CurrentUrl = _allowedUrls[0]; // Default to the first allowed URL (Piano) so the WebView shows a valid page on load.
        }

        // DependencyProperty registration for `CurrentUrl` so it can be used in XAML bindings and participate in WPF property system.
        // The property name "CurrentUrl" matches the CLR wrapper below.
        public static readonly DependencyProperty CurrentUrlProperty =
            DependencyProperty.Register(
                "CurrentUrl",                 // Name of the property
                typeof(string),               // Property type
                typeof(ChordsControl),        // Owner type (this control class)
                new PropertyMetadata(null));  // Default metadata; initial value is null (we set it in ctor)

        // CLR wrapper for the dependency property for easier use in code.
        public string CurrentUrl
        {
            get => (string)GetValue(CurrentUrlProperty); // Read the value from the WPF dependency property store.
            set => SetValue(CurrentUrlProperty, value);  // Write the value to the dependency property store (triggers binding updates).
        }

        // NAVIGATION BUTTONS
        // These methods are intended to be hooked up to button Click events in XAML.
        // Each simply changes `CurrentUrl` to one of the allowed URLs, causing the WebView (if bound) to navigate.
        private void NavigatePiano(object sender, RoutedEventArgs e) => CurrentUrl = _allowedUrls[0];   // Switch to Piano page
        private void NavigateGuitar(object sender, RoutedEventArgs e) => CurrentUrl = _allowedUrls[1];  // Switch to Guitar page
        private void NavigateUkulele(object sender, RoutedEventArgs e) => CurrentUrl = _allowedUrls[2]; // Switch to Ukulele page

        // THE BAN LOGIC
        // Handler for WebView2's NavigationStarting event. This fires when a navigation is about to start.
        private void MyWebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // If the user clicks a link INSIDE the page that isn't in our allow-list, cancel the navigation.
            // Note: e.Uri is a string representing the destination URL; the check is exact-match against our list.
            if (!_allowedUrls.Contains(e.Uri))
            {
                e.Cancel = true; // Cancel the navigation so the WebView remains on the allowed page.
                
            }
        }

        // Called after navigation completes successfully. inject JavaScript to hide navigation elements
        // of the hosted site so users cannot see other site navigation options.
        private async void MyWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // JavaScript that will hide common site navigation elements (header, footer, sidebars, etc.).
            // This reduces UI elements that could contain links to disallowed pages.
            string hideElementsScript = @"
                (function() {
                    const selectors = ['header', 'footer', '.site-nav', '.more-tools', '.sidebar', '.mobile-nav-toggle'];
                    selectors.forEach(selector => {
                        const el = document.querySelector(selector);
                        if (el) el.style.display = 'none';
                    });
                })();";

            // Execute the JavaScript in the context of the currently loaded page.
            // ExecuteScriptAsync runs asynchronously and does not block the UI thread.
            await MyWebView.ExecuteScriptAsync(hideElementsScript);
        }


    }
}

