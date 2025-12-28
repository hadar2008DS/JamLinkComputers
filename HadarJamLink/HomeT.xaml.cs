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
using ClientSide;
using Model;
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

        private async Task  LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PersonList pList=await apiService.GetPerson();
            Person p = pList.Find(p =>
                p.PassW == passwordBox.Password
                && p.Username == usernameBox.Text
            );
            if (p != null)
            {

            }
        }

    }
}
