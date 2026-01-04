using ClientSide;
using Model;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace JamLinkComputers
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        ApiService apiService = new ApiService();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validate inputs
            if (!ValidateInputs(out string username, out string password))
                return;

            string userType = ((ComboBoxItem)userTypeCombo.SelectedItem).Content.ToString();

            try
            {
                // 2. Create new Person object
                Person newUser = new Person
                {
                    Username = username,
                    PassW = password
                };

                // 3. Send to API
                await apiService.InsertPerson(newUser);

                MessageBox.Show("Registration successful!");

                // 4. Navigate to UserHomePage
                NavigationService?.Navigate(new UserHomePage());

                //// 4. Navigate to the appropriate home page
                //if (userType == "Musician")
                //    NavigationService?.Navigate(new MusicianHomePage(newUser, userType));
                //else
                //    NavigationService?.Navigate(new ProducerHomePage(newUser, userType));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering user: " + ex.Message);
            }
        }

        private bool ValidateInputs(out string username, out string password)
        {
            usernameError.Text = "";
            passwordError.Text = "";

            username = usernameBox.Text ?? string.Empty;
            password = passwordBox.Visibility == Visibility.Visible ? passwordBox.Password : passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                usernameError.Text = "נדרש שם משתמש.";
                usernameBox.Focus();
                return false;
            }

            if (username.Length > 20 || !Regex.IsMatch(username, @"^[A-Za-z0-9]+$"))
            {
                usernameError.Text = "שם המשתמש חייב להיות עד 20 תווים (אותיות/מספרים).";
                usernameBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                passwordError.Text = "נדרש סיסמה.";
                passwordBox.Focus();
                return false;
            }

            if (password.Length > 20 || !Regex.IsMatch(password, @"^[A-Za-z0-9]+$"))
            {
                passwordError.Text = "הסיסמה חייבת להיות עד 20 תווים (אותיות/מספרים).";
                passwordBox.Focus();
                return false;
            }

            return true;
        }

        // Password show/hide
        private void ShowPasswordCheck_Checked(object sender, RoutedEventArgs e)
        {
            passwordTextBox.Text = passwordBox.Password;
            passwordTextBox.Visibility = Visibility.Visible;
            passwordBox.Visibility = Visibility.Collapsed;
        }

        private void ShowPasswordCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = passwordTextBox.Text;
            passwordBox.Visibility = Visibility.Visible;
            passwordTextBox.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (showPasswordCheck.IsChecked == true)
                passwordTextBox.Text = passwordBox.Password;
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (showPasswordCheck.IsChecked == true)
                passwordBox.Password = passwordTextBox.Text;
        }
    }
}
