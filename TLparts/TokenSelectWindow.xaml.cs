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

namespace Suiren.TLparts
{
    /// <summary>
    /// TokenSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TokenSelectWindow : Window
    {
        private List<Tokens> Tokens;
        public Tokens SelectedToken;

        public TokenSelectWindow(List<Tokens> tokens, List<User> users)
        {
            InitializeComponent();
            Tokens = tokens;
            listBox.ItemsSource = users.Select(u => new TokenView(u));
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex < 0)
            {
                MessageBox.Show("アカウントを選択してください");
                return;
            }
            SelectedToken = Tokens[listBox.SelectedIndex];
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
