using ClientSide;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Xml.Linq;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for GroupsV.xaml
    /// </summary>
    public partial class GroupsV : UserControl
    {
        ApiService ApiService = new ApiService();
        List<Model.Group> allGroups = new List<Model.Group>();
        public GroupsV()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // 1. משיכת הנתונים המעודכנים ביותר מהשרת
                var gList = await ApiService.GetGroups();
                allGroups = gList.ToList();

                // 2. ניתוק וחיבור מחדש של המקור (חשוב לרענון ויזואלי)
                MyGroupsListBox.ItemsSource = null;
                MyGroupsListBox.ItemsSource = allGroups.Where(g => g.IsActive).ToList();

                AllGroupsListBox.ItemsSource = null;
                AllGroupsListBox.ItemsSource = allGroups.Where(g => !g.IsActive).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת נתונים: " + ex.Message);
            }
        }

        // אפשרות עריכה לקבוצות של המשתמש
        private async void EditGroup_Click(object sender, RoutedEventArgs e)
        {
            var group = (sender as Button).Tag as Model.Group;
            if (group == null) return;

            string newName = Microsoft.VisualBasic.Interaction.InputBox("Edit Group Name:", "Edit", group.GroupName);

            if (!string.IsNullOrWhiteSpace(newName) && newName != group.GroupName)
            {
                // 1. עדכון מקומי זמני
                string oldName = group.GroupName;
                group.GroupName = newName;

                // 2. שליחה לשרת - חשוב מאוד!
                int result = await ApiService.UpdateGroup(group);

                if (result > 0)
                {
                    MessageBox.Show("Name updated!");
                    LoadData(); // טעינה מחדש מהמסד כדי לוודא סנכרון מלא
                }
                else
                {
                    MessageBox.Show("Failed to update database.");
                    group.GroupName = oldName; // החזרה לשם הקודם אם נכשל
                }
            }
        }

        private async void JoinGroup_Click(object sender, RoutedEventArgs e)
        {
            // 1. חילוץ האובייקט של הקבוצה מהכפתור
            var btn = sender as Button;
            var group = btn.Tag as Model.Group;

            if (group == null) return;

            // 2. בדיקה אופציונלית - האם הקבוצה כבר פעילה? (למניעת לחיצות כפולות)
            if (group.IsActive)
            {
                MessageBox.Show("You are already a member of this group.");
                return;
            }

            try
            {
                // 3. עדכון הסטטוס ל-True
                group.IsActive = true;

                // 4. שליחת העדכון למסד הנתונים דרך ה-API
                // אנחנו משתמשים באותה פונקציית Update ששימשה אותנו ב-Leave
                int result = await ApiService.UpdateGroup(group);

                if (result > 0)
                {
                    MessageBox.Show($"Successfully joined '{group.GroupName}'!");

                    // 5. רענון הנתונים - זה יגרום לקבוצה "לקפוץ" ללשונית My Groups
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Could not join the group. Please try again later.");
                    // החזרת המצב לקדמותו בזיכרון המקומי במקרה של כישלון
                    group.IsActive = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error joining group: " + ex.Message);
            }
        }

        private async void LeaveGroup_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var group = btn.Tag as Model.Group;

            if (group == null) return;

            // הצגת דיאלוג אישור
            var confirm = MessageBox.Show($"Are you sure you want to leave {group.GroupName}?", "Leave Group", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                // 1. שינוי הסטטוס ל-false
                group.IsActive = false;

                // 2. עדכון השרת (חובה להשתמש ב-await)
                int result = await ApiService.UpdateGroup(group);

                if (result > 0)
                {
                    // 3. רענון כל הממשק - זה יגרום לקבוצה לעבור ללשונית "All Groups"
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Failed to update the server. Please try again.");
                    group.IsActive = true; // החזרת המצב בזיכרון למקרה של כישלון
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text.ToLower();
            MyGroupsListBox.ItemsSource = allGroups.Where(g => g.IsActive && g.GroupName.ToLower().Contains(search)).ToList();
            AllGroupsListBox.ItemsSource = allGroups.Where(g => !g.IsActive && g.GroupName.ToLower().Contains(search)).ToList();
        }

        private async void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            // 1. קבלת שם הקבוצה מהמשתמש
            string groupName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the name of the new group:",
                "Create New Group",
                "New Group Name");

            // 2. בדיקת תקינות
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return;
            }

            try
            {
                    GroupName = groupName,
                    CreationDate = DateTime.Now,
                    IsActive = true
                };

                // 4. שליחה ל-API - עכשיו ה-await יעבוד בלי שגיאה
                int newId = await ApiService.InsertGroup(newGroup);

                if (newId > 0)
                {
                    newGroup.Id = newId; // נותנים לו את ה-ID שחזר

                    // הוספה ידנית לרשימה בזיכרון
                    allGroups.Add(newGroup);

                    // עדכון ה-UI ללא תלות בטעינה מהשרת (לבדיקה)
                    MyGroupsListBox.ItemsSource = null;
                    MyGroupsListBox.ItemsSource = allGroups.Where(g => g.IsActive).ToList();

                    MessageBox.Show("Group Created!");
                }
                else
                {
                    MessageBox.Show("Failed to create the group. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }


        }

    }
}
