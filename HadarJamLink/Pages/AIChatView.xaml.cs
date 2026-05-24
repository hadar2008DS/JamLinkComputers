using MyAIAgent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace JamLinkComputers.Pages
{
    /// <summary>
    /// Interaction logic for AIChatView.xaml
    /// </summary>
    public partial class AIChatView : Page
    {
        private readonly AIEngine aiEngine;
        public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();
        //protected static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
        //              + System.IO.Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location
        //              + "/../../../../../ViewModel/JamLinkAccessDB.accdb");


        //API KEY from Groq: https://console.groq.com/settings/billing/plans
        public AIChatView()
        {
            InitializeComponent();
            ChatItemsControl.ItemsSource = Messages;

            // שחזור הנתיב בתוך ה-UI כדי לא לשנות את BaseDB
            string dbPath = System.IO.Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location
                           + "/../../../../../ViewModel/JamLinkAccessDB.accdb");
            string connString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

            // Start Engine
            aiEngine = new AIEngine(connString);
        }
        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            await ProcessMessage();
        }

        private void TxtUserInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnSend_Click(sender, e);
            }
        }

        private async Task ProcessMessage()
        {
            string userText = TxtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(userText)) return;

            // הוספת הודעת המשתמש למסך
            Messages.Add(new ChatMessage
            {
                Message = userText,
                Alignment = System.Windows.HorizontalAlignment.Right,
                BackgroundColor = "#DCF8C6"
            });

            TxtUserInput.Clear();

            try
            {
                // קריאה לפעולה הנכונה ב-DLL
                string response = await aiEngine.SendMessageAsync(userText);

                // הצגת תשובת האג'נט שחזר מה-DLL
                Messages.Add(new ChatMessage
                {
                    Message = response,
                    Alignment = System.Windows.HorizontalAlignment.Left,
                    BackgroundColor = "#F0F0F0"
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("שגיאה בתקשורת: " + ex.Message);
            }

            ChatScrollViewer.ScrollToBottom();
        }
    }
}
    // מחלקת עזר לייצוג הודעה ב-UI
    public class ChatMessage
    {
        public string Message { get; set; }
        public System.Windows.HorizontalAlignment Alignment { get; set; }
        public string BackgroundColor { get; set; }
    }


    



