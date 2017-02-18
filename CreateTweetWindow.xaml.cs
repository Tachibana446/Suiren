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
    /// CreateTweetWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CreateTweetWindow : Window
    {
        private List<Tokens> tokens;
        private List<User> users;
        private TweetPanel toReply;

        /// <summary>
        /// toReplyでReply先の設定
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="users"></param>
        /// <param name="toReply"></param>
        public CreateTweetWindow(List<Tokens> tokens, List<User> users, TweetPanel toReply = null)
        {
            this.toReply = toReply;
            this.tokens = tokens;
            this.users = users;
            InitializeComponent();
            comboBox.ItemsSource = users.Select(u => $"{u.Name}(@{u.ScreenName})");
            comboBox.SelectedIndex = 0;
            if (toReply != null)
            {
                reply.Children.Add(toReply);
                textBox.Text = $"@{toReply.Tweet.UserScreenName} ";
            }
        }

        private void TweetButton_Click(object sender, RoutedEventArgs e)
        {
            Tokens token = null;
            try { token = tokens[comboBox.SelectedIndex]; }
            catch
            {
                MessageBox.Show("つぶやくアカウントを選択してください");
                return;
            }
            var text = textBox.Text.Trim();
            if (text.Length == 0)
            {
                MessageBox.Show("ツイートが空です");
                return;
            }
            else if (text.Length > 140)
            {
                MessageBox.Show("ツイートが１４０字を超えています");
                return;
            }
            try
            {
                if (toReply != null)
                    token.Statuses.Update(status: text, in_reply_to_status_id: toReply.Tweet.Id);
                else
                    token.Statuses.Update(status: text);
                MessageBox.Show("つぶやきました");
                textBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
