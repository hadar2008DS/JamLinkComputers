using System;
using System.Collections.Generic;
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
using Microsoft.VisualBasic;
using Model;
using ClientSide;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for ProjectWorkspaceV.xaml
    /// </summary>
    public partial class ProjectWorkspaceV : UserControl
    {
        private Group currentGroup;
        private Person currentUser;
        public ProjectWorkspaceV(Group group, Person user)
        {
            InitializeComponent();
            this.currentGroup = group;
            this.currentUser = user;
            this.Loaded += (s,e) => {
                if (this.currentGroup == null) {
                    // handle gracefully: disable controls or load group by id
                    CurrentTrackTextBlock.Text = "No group selected";
                    return;
                }
                RefreshScreen();
            };
        }

        private void RefreshScreen()
        {
            
            var allTasks = WorkspaceDataManager.GetTasks(currentGroup.Id);

            PendingTasksControl.ItemsSource = null;
            PendingTasksControl.ItemsSource = allTasks.Where(t => !t.IsCompleted).ToList();

            CompletedTasksControl.ItemsSource = null;
            CompletedTasksControl.ItemsSource = allTasks.Where(t => t.IsCompleted).ToList();

            SuggestionsControl.ItemsSource = null;
            SuggestionsControl.ItemsSource = WorkspaceDataManager.GetSuggestions(currentGroup.Id);

            //Project in Line
            var projectQueue = WorkspaceDataManager.GetProjectQueue(currentGroup.Id);

            if (projectQueue.Count > 0)
            {
                //First in queue
                CurrentTrackTextBlock.Text = "🎵 Current Track: " + projectQueue.Peek();
                ProjectProgressBar.Value = 75; // מד התקדמות זמני לשיר הפעיל
                NextTracksListBox.Visibility = Visibility.Visible;
            }
            else
            {
                CurrentTrackTextBlock.Text = "🎵 No tracks in queue. Add a new project!";
                ProjectProgressBar.Value = 0;
                NextTracksListBox.Visibility = Visibility.Collapsed;
            }

            // Other songs in line
            NextTracksListBox.ItemsSource = null;
            NextTracksListBox.ItemsSource = projectQueue.Skip(1).ToList(); // מדלג על הנוכחי ומציג את השאר
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string taskTitle = Interaction.InputBox("Enter the task description:", "New Task", "e.g., Record guitar solo");
            if (string.IsNullOrWhiteSpace(taskTitle) || taskTitle == "e.g., Record guitar solo") return;

            var tasksList = WorkspaceDataManager.GetTasks(currentGroup.Id);
            tasksList.Add(new GroupTask
            {
                Title = taskTitle,
                AssignedTo = "Assigned to: " + currentUser.Username,
                IsCompleted = false
            });

            RefreshScreen();
        }

        private void TaskStatus_Changed(object sender, RoutedEventArgs e)
        {
            RefreshScreen();
        }

        private void PostSuggestion_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewSuggestionTextBox.Text)) return;

            var suggestionsList = WorkspaceDataManager.GetSuggestions(currentGroup.Id);
            suggestionsList.Add(new GroupSuggestion
            {
                AuthorName = currentUser.Username,
                Content = NewSuggestionTextBox.Text,
                DatePosted = DateTime.Now
            });

            NewSuggestionTextBox.Clear();
            RefreshScreen();
        }

        //Queue to control adding new song
        private void AddProjectToQueue_Click(object sender, RoutedEventArgs e)
        {
            string trackName = Interaction.InputBox("Enter the name of the new track/project:", "Add to Queue", "e.g., Album Track #2");
            if (string.IsNullOrWhiteSpace(trackName) || trackName == "e.g., Album Track #2") return;

            var queue = WorkspaceDataManager.GetProjectQueue(currentGroup.Id);
            queue.Enqueue(trackName); // insert to queue

            RefreshScreen(); // עדכון ה-UI
        }

        // Progress to other song
        private void NextProject_Click(object sender, RoutedEventArgs e)
        {
            var queue = WorkspaceDataManager.GetProjectQueue(currentGroup.Id);

            if (queue.Count > 0)
            {
                string completedTrack = queue.Dequeue(); // הוצאה של השיר הנוכחי מהתור
                MessageBox.Show($"Great job! '{completedTrack}' has been moved out of the working queue.");
                RefreshScreen(); // עדכון ה-UI
            }
        }
    }

    //Helper classes
    public class GroupTask
    {
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class GroupSuggestion
    {
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
    }

    //DATA MANAGER that has a queue for every group

    public static class WorkspaceDataManager
    {
        private static Dictionary<int, List<GroupTask>> GroupTasksDic = new Dictionary<int, List<GroupTask>>();
        private static Dictionary<int, List<GroupSuggestion>> GroupSuggestionsDic = new Dictionary<int, List<GroupSuggestion>>();

        // New Dictionary to help the queue
        private static Dictionary<int, Queue<string>> GroupProjectQueueDic = new Dictionary<int, Queue<string>>();

        public static List<GroupTask> GetTasks(int groupId)
        {
            if (!GroupTasksDic.ContainsKey(groupId)) GroupTasksDic[groupId] = new List<GroupTask>();
            return GroupTasksDic[groupId];
        }

        public static List<GroupSuggestion> GetSuggestions(int groupId)
        {
            if (!GroupSuggestionsDic.ContainsKey(groupId)) GroupSuggestionsDic[groupId] = new List<GroupSuggestion>();
            return GroupSuggestionsDic[groupId];
        }

        // helper function to get the wanted group 
        public static Queue<string> GetProjectQueue(int groupId)
        {
            if (!GroupProjectQueueDic.ContainsKey(groupId))
            {
                var newQueue = new Queue<string>();
                // Adding a defult song
                newQueue.Enqueue("JamLink Album Session #1 (Mastering)");
                GroupProjectQueueDic[groupId] = newQueue;
            }
            return GroupProjectQueueDic[groupId];
        }
    }
}
