using ClientSide;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for GroupsV.xaml
    /// </summary>
    public partial class GroupsV : UserControl
    {

        ApiService apiService = new ApiService();
        Person currentUser;
        private List<Model.Group> allAvailableGroups;
        public GroupsV(Person loggedInUser)
        {
            InitializeComponent();
            this.currentUser = loggedInUser;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                GroupList allGroups = await apiService.GetGroups();
                GroupMembersList allGroupMembers = await apiService.GetGroupMembers();

                var myGroupMembers = allGroupMembers
                    .Where(gm => gm.Id == currentUser.Id)
                    .ToList();

                var myGroupIds = myGroupMembers
                    .Select(gm => gm.Group.Id)
                    .ToList();

                allAvailableGroups = allGroups
                    .Where(g => !myGroupIds.Contains(g.Id))
                    .ToList();

                MyGroupsListBox.ItemsSource = myGroupMembers;
                AvailableGroupsListBox.ItemsSource = allAvailableGroups;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading groups: " + ex.Message);
            }
        }

        // אפשרות עריכה לקבוצות של המשתמש
        private async void EditGroup_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            GroupMembers gm = btn?.DataContext as GroupMembers;

            if (gm == null)
                return;

            try
            {
                await apiService.UpdateGroupAsync(gm.Group);
                MessageBox.Show("Group updated successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update group: " + ex.Message);
            }
        }

        private async void JoinGroup_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Model.Group group = btn?.DataContext as Model.Group;

            if (group == null)
                return;

            try
            {
                GroupMembers gm = new GroupMembers
                {
                    Id = currentUser.Id,
                    Group = group
                };

                await apiService.InsertGroupMember(gm);
                MessageBox.Show("Joined group successfully!");

                LoadData(); // רענון
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to join group: " + ex.Message);
            }
        }

        private async void LeaveGroup_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            GroupMembers gm = btn?.DataContext as GroupMembers;

            if (gm == null)
                return;

            try
            {
                await apiService.DeleteGroupMember(gm.Id);
                MessageBox.Show("You left the group.");

                LoadData(); // רענון כמו אחרי Login
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to leave group: " + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (allAvailableGroups == null)
                return;

            string text = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(text))
            {
                AvailableGroupsListBox.ItemsSource = allAvailableGroups;
            }
            else
            {
                var filtered = allAvailableGroups
                    .Where(g => g.GroupName != null &&
                                g.GroupName.ToLower().Contains(text))
                    .ToList();

                AvailableGroupsListBox.ItemsSource = filtered;
            }
        }

        private async void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            //string groupName = NewGroupNameBox.Text?.Trim();

            //// 1️⃣ בדיקות קלט (כמו Login)
            //if (string.IsNullOrWhiteSpace(groupName))
            //{
            //    MessageBox.Show("Please enter a group name.");
            //    NewGroupNameBox.Focus();
            //    return;
            //}

            //if (groupName.Length > 30)
            //{
            //    MessageBox.Show("Group name must be at most 30 characters.");
            //    NewGroupNameBox.Focus();
            //    return;
            //}

            //try
            //{
            //    // 2️⃣ יצירת אובייקט Group
            //    Model.Group newGroup = new Model.Group
            //    {
            //        GroupName = groupName,
            //        CreationDate = DateTime.Now,
            //        IsActive = true
            //    };

            //    // 3️⃣ שליחה לשרת
            //    int result = await apiService.InsertGroup(newGroup);

            //    if (result <= 0)
            //    {
            //        MessageBox.Show("Failed to create group.");
            //        return;
            //    }

            //    MessageBox.Show("Group created successfully!");

            //    // 4️⃣ ניקוי שדה
            //    NewGroupNameBox.Text = string.Empty;

            //    // 5️⃣ רענון הרשימות
            //    LoadData();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error creating group: " + ex.Message);
            //}
        }

        private void ViewGroupDetails_Click(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void UpdateGroup_Click(object sender, RoutedEventArgs e)
        {
            //// בדיקה שנבחרה קבוצה
            //if (AllGroupsListBox.SelectedItem == null)
            //{
            //    MessageBox.Show("בחר קבוצה לעדכון");
            //    return;
            //}

            //// בדיקה שהשם לא ריק
            //if (string.IsNullOrWhiteSpace(GroupNameTextBox.Text))
            //{
            //    MessageBox.Show("שם קבוצה לא יכול להיות ריק");
            //    return;
            //}

            //// הקבוצה שנבחרה
            //Group selectedGroup = (Group)AllGroupsListBox.SelectedItem;

            //// עדכון שדות
            //selectedGroup.GroupName = GroupNameTextBox.Text;
            //selectedGroup.IsActive = IsActiveCheckBox.IsChecked ?? false;

            //// רענון ה־ListBox
            //AllGroupsListBox.Items.Refresh();

            //MessageBox.Show("הקבוצה עודכנה בהצלחה");
        }
    }
}
