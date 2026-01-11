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
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        ApiService apiService = new ApiService();
        public LogInPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            loginButton.IsEnabled = false; // prevent multiple clicks

            // 1 Check inputs
            if (!ValidateLoginInputs(out string username, out string password))
                return;

            try
            {
                // 2 call API to get all users
                PersonList pList = await apiService.GetPeople();

                if (pList == null || pList.Count == 0)
                {
                    passwordError.Text = "No users returned from server.";
                    return;
                }

                // 3  If user exists with matching username and password  
                Person p = pList.Find(u =>
                    u.Username == username &&
                    u.PassW == password
                );

                if (p == null)
                {
                    passwordError.Text = "Invalid username or password.";
                    return;
                }

                // 4 Login successful
                passwordError.Text = "";
                usernameError.Text = "";
                MessageBox.Show("Login successful!");

                // 5 Navigate to Loading (pass username so Loading can forward to UserHomePage)
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new UserHome(p));
                }
                else
                {
                    MessageBox.Show("NavigationService is null. Make sure LogInPage is inside a Frame!");
                }
            }
            catch (Exception ex)
            {
                // Hendle Connection error ( JSON null/not valid )
                passwordError.Text = "Error connecting to server: " + ex.Message;
            }
            finally
            {
                loginButton.IsEnabled = true; // re-enable button
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

                var nav = NavigationService;

                if (nav == null)
                {
                    MessageBox.Show("Navigation failed.");
                    return;
                }

                nav.Navigate(registerPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Register page: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
