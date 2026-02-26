using ClientSide;
using Melanchall.DryWetMidi.Tools;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for GroupsV.xaml
    /// </summary>
    public partial class GroupsV : UserControl
    {
        private ApiService _apiService = new ApiService();
        private Person _currentUser;
        private List<Model.Group> _allAvailableGroupsRaw;

        public GroupsV(Person loggedInUser)
        {
            InitializeComponent();
            _currentUser = loggedInUser;

            // Trigger the initial data fetch once the UI is ready
            this.Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                // Fetch both lists from the API
                var allGroups = await _apiService.GetGroups();
                var allMemberships = await _apiService.GetGroupMembers();

                // 1. Get memberships belonging to the current user
                var myMemberships = allMemberships
                    .Where(m => m.Id == _currentUser.Id || (m.Username == _currentUser.Username))
                    .ToList();

                // 2. Get the IDs of groups the user is already in
                var myGroupIds = myMemberships
                    .Select(m => m.Group?.Id)
                    .Where(id => id != null)
                    .ToList();

                // 3. Filter groups for the 'Available' tab (only those NOT in myGroupIds)
                _allAvailableGroupsRaw = allGroups
                    .Where(g => !myGroupIds.Contains(g.Id))
                    .ToList();

                // 4. Bind to the UI
                MyGroupsListBox.ItemsSource = myMemberships;
                AvailableGroupsListBox.ItemsSource = _allAvailableGroupsRaw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Refresh failed: {ex.Message}");
            }
        }

        private async void JoinGroup_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Model.Group selectedGroup)
            {
                try
                {
                    // FIX: Including Username and PassW to satisfy API requirements
                    var newMembership = new GroupMembers
                    {
                        Id = _currentUser.Id,
                        Username = _currentUser.Username,
                        PassW = _currentUser.PassW,
                        Group = selectedGroup
                    };

                    await _apiService.InsertGroupMember(newMembership);

                    // This is the key: Re-run LoadData to move the item to the "My Groups" tab
                    await LoadData();

                    MessageBox.Show($"You joined {selectedGroup.GroupName}!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not join: {ex.Message}");
                }
            }
        }

        private async void LeaveGroup_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is GroupMembers membership)
            {
                try
                {
                    await _apiService.DeleteGroupMember(membership.Id);

                    // Re-run LoadData to move the item back to "Available Groups"
                    await LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error leaving group: {ex.Message}");
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allAvailableGroupsRaw == null) return;

            string query = SearchBox.Text.ToLower().Trim();

            // If the search is empty or the placeholder text, show everything
            if (string.IsNullOrEmpty(query) || query == "search groups...")
            {
                AvailableGroupsListBox.ItemsSource = _allAvailableGroupsRaw;
            }
            else
            {
                AvailableGroupsListBox.ItemsSource = _allAvailableGroupsRaw
                    .Where(g => g.GroupName.ToLower().Contains(query))
                    .ToList();
            }
        }


        private async void UpdateGroup_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is GroupMembers gm && gm.Group != null)
            {
                string currentName = gm.Group.GroupName;

                string newName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter new group name:",
                    "Update Group",
                    currentName);

                if (string.IsNullOrWhiteSpace(newName) || newName == currentName)
                    return;

                try
                {
                    var updatedGroup = new Model.Group();
                    updatedGroup.Id = gm.Group.Id;
                    updatedGroup.GroupName = newName;

                    //IMPORTANT: Only send what API needs
                    await _apiService.UpdateGroupAsync(updatedGroup);

                    await LoadData();

                    MessageBox.Show("Group updated successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Update failed: {ex.Message}");
                }
            }
        }


    }
}
