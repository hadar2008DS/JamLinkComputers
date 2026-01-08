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

            string userType = "";

            if (userTypeCombo.SelectedItem != null)
            {
                ComboBoxItem item = (ComboBoxItem)userTypeCombo.SelectedItem;
                userType = item.Content.ToString();
            }


            try
            {
                // 2. Create new Person object
                Person newUser = new Person
                {
                    Username = username,
                    PassW = password
                };

                // 3. Send to API - insert base person record
                await apiService.InsertPerson(newUser);

                // Also insert into specific role table if applicable
                // If ApiService exposes InsertMusician/InsertProducer that accept Person (or similar),
                // these calls will attach role-specific records. If it differ adjust accordingly.
                try
                {
                    // compare the userType. I used Var to simplify the code.
                    string utype = "";

                    if (userType != null)
                    {
                        utype = userType.ToLower();// used ToLower() to avoid case sensitivity issues
                    }


                    if (utype == "musician")
                    {
                        // Create a Musician object from Person
                        Musician newMusician = new Musician
                        {
                            Username = newUser.Username,
                            PassW = newUser.PassW,
                            IsActive = true // set as needed, can be modified later
                            // Add other Musician specific properties if required
                        };
                        await apiService.InsertMusician(newMusician);
                    }
                    else if (utype == "producer")
                    {
                        // Create a Producer object from Person
                        Producer newProducer = new Producer
                        {
                            Username = newUser.Username,
                            PassW = newUser.PassW,
                            IsActive = true // set as needed, can be modified later
                            // Add other Producer-specific properties if required
                        };
                        await apiService.InsertProducer(newProducer);
                    }
                }
                catch (Exception roleEx)
                {
                    // Role specific insert failed then notify but allow registration to proceed for base Person.
                    MessageBox.Show("Registered user, but role registration failed: " + roleEx.Message);
                }

                MessageBox.Show("Registration successful!");

                // 4. Navigate to Loading screen (shows overlay then navigates to UserHomePage)
                NavigationService?.Navigate(new UserHome(newUser.Id));
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
            

            if (passwordBox.Visibility == Visibility.Visible)
            {
                password = passwordBox.Password;
            }
            else
            {
                password = passwordTextBox.Text;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                usernameError.Text = "Username is required.";
                usernameBox.Focus();
                return false;
            }

            if (username.Length > 20 || !Regex.IsMatch(username, @"^[A-Za-z0-9]+$"))
            {
                usernameError.Text = "Username must be at most 20 characters and contain only letters and digits.";
                usernameBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                passwordError.Text = "Password is required.";
                if (passwordBox.Visibility == Visibility.Visible)
                    passwordBox.Focus();
                else
                    passwordTextBox.Focus();
                return false;
            }

            if (password.Length > 20 || !Regex.IsMatch(password, @"^[A-Za-z0-9]+$"))
            {
                passwordError.Text = "Password must be at most 20 characters and contain only letters and digits.";
                if (passwordBox.Visibility == Visibility.Visible)
                    passwordBox.Focus();
                else
                    passwordTextBox.Focus();
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
