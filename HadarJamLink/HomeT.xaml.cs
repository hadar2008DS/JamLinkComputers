using ClientSide;
using JamLinkComputers;
using Model;
using System;
using System.Collections.Generic;
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
namespace HadarJamLink
{
    /// <summary>
    /// Interaction logic for HomeT.xaml
    /// </summary>
    public partial class HomeT : Page
    {
        ApiService apiService = new ApiService();
        public HomeT()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
            // 1 בדיקת קלטים
            if (!ValidateLoginInputs(out string username, out string password))
                return;

            try
            {
                // 2 קריאה ל-API דרך ApiService
                PersonList pList = await apiService.GetPeople();

                if (pList == null || pList.Count == 0)
                {
                    passwordError.Text = "No users returned from server.";
                    return;
                }

                // 3 בדיקה אם המשתמש קיים  
                Person p = pList.Find(u =>
                    u.Username == username &&
                    u.PassW == password
                );

                if (p == null)
                {
                    passwordError.Text = "Invalid username or password.";
                    return;
                }

                // 4 כניסה הצליחה
                passwordError.Text = "";
                usernameError.Text = "";
                MessageBox.Show("Login successful!");

                // 5 מעבר לעמוד הבית
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new UserHome());
                }
                else
                {
                    MessageBox.Show("NavigationService is null. Make sure HomeT is inside a Frame!");
                }
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות חיבור / JSON ריק
                passwordError.Text = "Error connecting to server: " + ex.Message;
            }
        }


        private void ShowPasswordCheck_Checked(object sender, RoutedEventArgs e)
        {
            // show plain text and copy current password
            passwordTextBox.Text = passwordBox.Password;
            passwordTextBox.Visibility = Visibility.Visible;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordTextBox.Focus();
            passwordTextBox.Select(passwordTextBox.Text.Length, 0);
        }

        private void ShowPasswordCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            // hide plain text and copy back to PasswordBox
            passwordBox.Password = passwordTextBox.Text;
            passwordTextBox.Visibility = Visibility.Collapsed;
            passwordBox.Visibility = Visibility.Visible;
            passwordBox.Focus();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // keep plaintext textbox in sync while visible
            if (showPasswordCheck.IsChecked == true)
                passwordTextBox.Text = passwordBox.Password;
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // keep password box in sync while hidden (so login reads correct value)
            if (showPasswordCheck.IsChecked == true)
                passwordBox.Password = passwordTextBox.Text;
        }


    private bool ValidateLoginInputs(out string username, out string password)
        {
            // clear previous errors
            usernameError.Text = string.Empty;
            passwordError.Text = string.Empty;

            // get username
            username = usernameBox.Text ?? string.Empty;

            // username validation
            if (string.IsNullOrWhiteSpace(username))
            {
                usernameError.Text = "Username is required.";
                usernameBox.Focus();
                password = null;
                return false;
            }

            if (username.Length > 20)
            {
                usernameError.Text = "Username must be at most 20 characters.";
                usernameBox.Focus();
                password = null;
                return false;
            }

            if (!Regex.IsMatch(username, @"^[A-Za-z0-9]+$"))
            {
                usernameError.Text = "Username must contain only English letters and digits.";
                usernameBox.Focus();
                password = null;
                return false;
            }

            // get password (from the visible control)
            password = passwordBox.Visibility == Visibility.Visible
                ? passwordBox.Password
                : passwordTextBox.Text;

            // password validation
            if (string.IsNullOrWhiteSpace(password))
            {
                passwordError.Text = "Password is required.";
                if (passwordBox.Visibility == Visibility.Visible)
                    passwordBox.Focus();
                else
                    passwordTextBox.Focus();
                return false;
            }

            if (password.Length > 20)
            {
                passwordError.Text = "Password must be at most 20 characters.";
                passwordBox.Focus();
                return false;
            }

            if (!Regex.IsMatch(password, @"^[A-Za-z0-9]+$"))
            {
                passwordError.Text = "Password must contain only letters and digits.";
                passwordBox.Focus();
                return false;
            }

            // all inputs are valid
            return true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous errors
            usernameError.Text = string.Empty;
            passwordError.Text = string.Empty;

            try
            {
                var registerPage = new RegisterPage();

                // Try to get a NavigationService (prefer this.NavigationService)
                var nav = NavigationService ?? System.Windows.Navigation.NavigationService.GetNavigationService(this);

                if (nav != null)
                {
                    nav.Navigate(registerPage);
                    return;
                }

                // Helpful message if navigation isn't available
                MessageBox.Show(
                    "Unable to navigate to RegisterPage. Ensure this page is hosted inside a Frame or NavigationWindow.",
                    "Navigation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Register page: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
