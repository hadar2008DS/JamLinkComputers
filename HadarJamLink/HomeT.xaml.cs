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
            //// 1. validate inputs
            //if (!ValidateLoginInputs(out string username, out string password))
            //    return;

            //// 2. check against server / DB
            //PersonList pList = await apiService.GetPerson();

            //Person p = pList.Find(p =>
            //    p.Username == username &&
            //    p.PassW == password
            //);

            //if (p == null)
            //{
            //    passwordError.Text = "Invalid username or password.";
            //    return;
            //}

            //// 3. login success
            //MessageBox.Show("Login successful!", "Success");

            //// 4. navigate to user home page
            //if (NavigationService != null)
            //{
            //    NavigationService.Navigate(new UserHome());
            //}
            //else
            //{
            //    MessageBox.Show("NavigationService is null");
            //}
            //MessageBox.Show("1. Click entered");

            //if (!ValidateLoginInputs(out string username, out string password))
            //{
            //    MessageBox.Show("2. Validation failed");
            //    return;
            //}

            //MessageBox.Show("3. Validation OK");

            ////PersonList pList = await apiService.GetPerson();
            //PersonList pList = null;

            //try
            //{
            //    pList = await apiService.GetPerson();
            //    MessageBox.Show("4. Got users: " + (pList?.Count ?? 0));
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error in GetPerson(): " + ex.Message);
            //}


            //MessageBox.Show("4. Got users from API");

            //Person p = pList.Find(p =>
            //    p.Username == username &&
            //    p.PassW == password
            //);

            //if (p == null)
            //{
            //    MessageBox.Show("5. User not found");
            //    return;
            //}

            //MessageBox.Show("6. Login successful");

            //NavigationService?.Navigate(new UserHome());
            // 1️⃣ בדיקת קלטים
            if (!ValidateLoginInputs(out string username, out string password))
                return;

            try
            {
                // 2️⃣ קריאה ל-API דרך ApiService
                PersonList pList = await apiService.GetPeople();

                if (pList == null || pList.Count == 0)
                {
                    passwordError.Text = "No users returned from server.";
                    return;
                }

                // 3️⃣ בדיקה אם המשתמש קיים (ללא Trim)
                Person p = pList.Find(u =>
                    u.Username == username &&
                    u.PassW == password
                );

                if (p == null)
                {
                    passwordError.Text = "Invalid username or password.";
                    return;
                }

                // 4️⃣ כניסה הצליחה
                passwordError.Text = "";
                usernameError.Text = "";
                MessageBox.Show("Login successful!");

                // 5️⃣ מעבר לעמוד הבית
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

    }
}
