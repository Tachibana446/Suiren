using CoreTweet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Suiren
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private OAuth.OAuthSession Session = OAuth.Authorize(Keys.ConsumerKey, Keys.ConsumerSecret);
        private List<Tokens> _tokens = new List<CoreTweet.Tokens>();
        private List<Tokens> Tokens
        {
            get
            {
                return _tokens;
            }
            set
            {
                if (_tokens != value)
                {
                    _tokens = value;
                    authorizedCountMenu.Header = $"認証済み:{_tokens.Count}";
                }
            }
        }
        /// <summary>
        /// トークンごとに対応するユーザーデータ
        /// </summary>
        private List<User> UserAccounts = new List<User>();
        private int CurrentTokenIndex = 0;
        private AuthWindow AuthWindow = null;
        private List<Timeline> Panes = new List<Timeline>();

        // まとめて閉じるときに使うかも
        //private List<CreateTweetWindow> createTweetWindows = new List<CreateTweetWindow>();

        public MainWindow()
        {
            InitializeComponent();
            LoadTokens();

        }

        /// <summary>
        /// アカウントを認証する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void authMenu_Click(object sender, RoutedEventArgs e)
        {
            // Authトークンの再取得のためのセッション再認証
            Session = OAuth.Authorize(Session.ConsumerKey, Session.ConsumerSecret);

            AuthWindow = new AuthWindow(Session);
            AuthWindow.ShowDialog();
            if (!AuthWindow.isOk) return;
            Tokens token;
            statusBarText.Text = "アカウント認証中...";
            try
            {
                token = Session.GetTokens(AuthWindow.Pin);
            }
            catch
            {
                MessageBox.Show("入力されたPINが不正です。");
                statusBarText.Text = "";
                return;
            }
            Tokens.Add(token);
            UserAccounts.Add(token.Statuses.UserTimeline().First().User);
            SaveTokens();
            authorizedCountMenu.Header = $"認証済み:{Tokens.Count}";
            statusBarText.Text = "アカウント認証完了";
            authMenu.IsChecked = true;
        }

        /// <summary>
        /// ホームタイムラインの追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void homeTimelineMenu_Click(object sender, RoutedEventArgs e)
        {
            Tokens token = null;
            if (Tokens.Count <= 0)
            {
                MessageBox.Show("まずは認証してください");
                return;
            }
            else if (Tokens.Count == 1)
            {
                token = Tokens.First();
            }
            else
            {
                var dlg = new TLparts.TokenSelectWindow(Tokens, UserAccounts);
                if (dlg.ShowDialog() == true)
                    token = dlg.SelectedToken;
                else
                    return;
            }
            var pane = new TimelineSample(token, this);
            Panes.Add(pane);
            PanesControl.Items.Add(pane);
            ResizePanes();
            ReflectPanesBackground();
            await pane.LoadTimeline();
            return;
        }

        /// <summary>
        /// RTして保存する
        /// </summary>
        /// <param name="tweet"></param>
        public void CreateRetweet(Tweet tweet)
        {
            try
            {
                if (Tokens.Count == 0)
                    return;
                else if (Tokens.Count == 1)
                {
                    Tokens.First().Statuses.Retweet(tweet.Id);
                }
                else
                {
                    // アカウント選択
                    var window = new TLparts.TokenSelectWindow(Tokens, UserAccounts);
                    window.ShowDialog();
                    if (window.DialogResult == true)
                        window.SelectedToken.Statuses.Retweet(tweet.Id);
                    else
                        return;
                }
            }
            catch (TwitterException e)
            {
                MessageBox.Show(e.Message + "\n保存だけしました。");
            }
            // 保存
            tweet.Save();
        }

        /// <summary>
        /// いいねして保存する
        /// </summary>
        /// <param name="tweet"></param>
        public void CreateFavorite(Tweet tweet)
        {
            try
            {
                if (Tokens.Count == 0) return;
                else if (Tokens.Count == 1)
                    Tokens.First().Favorites.Create(id: tweet.Id);
                else
                {
                    // アカウント選択
                    var window = new TLparts.TokenSelectWindow(Tokens, UserAccounts);
                    window.ShowDialog();
                    if (window.DialogResult == true)
                        window.SelectedToken.Favorites.Create(id: tweet.Id);
                    else
                        return;
                }
            }
            catch (TwitterException e)
            {
                MessageBox.Show(e.Message + "\n保存だけしました");
            }
            // 保存
            tweet.Save();
        }

        private void SaveTokens()
        {
            using (var sw = new System.IO.StreamWriter("tokens.dat"))
            {
                foreach (var t in Tokens)
                {
                    sw.WriteLine(t.AccessToken);
                    sw.WriteLine(t.AccessTokenSecret);
                }
            }
        }

        private void LoadTokens()
        {
            if (!System.IO.File.Exists("tokens.dat")) return;
            var lines = System.IO.File.ReadLines("tokens.dat");
            if (lines.Count() == 0) return;
            bool isToken = true;
            string tokenStr = "";
            statusBarText.Text = "アカウントロード中...";
            statusProgressBar.Visibility = Visibility.Visible;
            statusProgressBar.Maximum = lines.Count();
            statusProgressBar.Value = 0;
            foreach (var line in lines)
            {
                if (isToken) { tokenStr = line; isToken = false; }
                else
                {
                    try
                    {
                        var token = CoreTweet.Tokens.Create(Keys.ConsumerKey, Keys.ConsumerSecret, tokenStr, line);
                        Tokens.Add(token);
                        UserAccounts.Add(token.Statuses.UserTimeline().First().User);
                    }
                    catch
                    {
                        continue;
                    }
                    isToken = true;
                }
                statusProgressBar.Value += 1;
            }
            authorizedCountMenu.Header = $"認証済み:{Tokens.Count}";
            statusBarText.Text = "アカウントロード完了";
            statusProgressBar.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// タイムラインのペインの高さをリサイズ
        /// </summary>
        private void ResizePanes()
        {
            foreach (Control item in PanesControl.Items)
            {
                item.MaxHeight = tabControl.ActualHeight - 30;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePanes();
        }

        /// <summary>
        /// 返信タイムラインを追加
        /// TODO この辺共通化できそう
        /// </summary>
        private async void MentionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Tokens token = null;
            if (Tokens.Count <= 0)
            {
                MessageBox.Show("まずは認証してください");
                return;
            }
            else if (Tokens.Count == 1)
            {
                token = Tokens.First();
            }
            else
            {
                var dlg = new TLparts.TokenSelectWindow(Tokens, UserAccounts);
                if (dlg.ShowDialog() == true)
                    token = dlg.SelectedToken;
                else
                    return;
            }
            var pane = new MentionsTimeline(token, this);
            Panes.Add(pane);
            PanesControl.Items.Add(pane);
            ResizePanes();
            ReflectPanesBackground();
            await pane.LoadTimeline();
            return;
        }

        /// <summary>
        /// 「つぶやく」メニュークリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateTweetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateTweetWindow(Tokens, UserAccounts);
            // もし開いたウィンドウを記憶しておきたければ
            //createTweetWindows.Add(window);
            window.Show();
        }


        private void logoutMenu_Click(object sender, RoutedEventArgs e)
        {
            var logoutWindow = new Logout(UserAccounts, this);
            logoutWindow.ShowDialog();
        }

        public void Logout(int index)
        {
            try
            {
                Tokens.RemoveAt(index);
                UserAccounts.RemoveAt(index);
                authorizedCountMenu.Header = $"認証済み:{Tokens.Count}";
                SaveTokens();
                MessageBox.Show("ログアウトしました");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowReplyWindow(TweetPanel toReply)
        {
            var window = new CreateTweetWindow(Tokens, UserAccounts, this, toReply);
            window.ShowDialog();
        }
        /// <summary>
        /// 「設定」メニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingWindow();
            window.ShowDialog();
            // 設定の反映
            normalTab.Background = Setting.Instance.BackgroundBrush;
            ReflectPanesBackground();
        }

        /// <summary>
        /// 各ペインの背景を設定したものに変更する
        /// </summary>
        private void ReflectPanesBackground()
        {
            foreach (UserControl pane in PanesControl.Items)
            {
                pane.Opacity = Setting.Instance.PaneOpacity;
                var paneType = pane.GetType();
                if (Setting.Instance.PaneBrushes.Any(pc => pc.PaneClass == paneType))
                    pane.Background = Setting.Instance.PaneBrushes.First(pc => pc.PaneClass == paneType).Brush;
            }
        }
    }
}
