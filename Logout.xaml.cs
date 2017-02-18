using CoreTweet;
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
using System.Windows.Shapes;

namespace Suiren
{
    /// <summary>
    /// Logout.xaml の相互作用ロジック
    /// </summary>
    public partial class Logout : Window
    {
        List<User> users = new List<User>();
        MainWindow parent;

        public Logout(List<User> users, MainWindow parent)
        {
            this.users = users;
            this.parent = parent;
            InitializeComponent();
            comboBox.ItemsSource = users.Select(u => $"{u.Name}(@{u.ScreenName})");
            comboBox.SelectedIndex = 0;
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            parent.Logout(comboBox.SelectedIndex);
        }
    }
}
