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
        private List<Tokens> Tokens = new List<CoreTweet.Tokens>();
        private AuthWindow AuthWindow = null;
        private List<TimelineSample> Panes = new List<TimelineSample>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void authMenu_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow = new AuthWindow(Session);
            AuthWindow.ShowDialog();
            if (!AuthWindow.isOk) return;
            Tokens token;
            try
            {
                token = Session.GetTokens(AuthWindow.Pin);
            }
            catch
            {
                MessageBox.Show("入力されたPINが不正です。");
                return;
            }
            Tokens.Add(token);
            authMenu.IsChecked = true;
        }

        private async void homeTimelineMenu_Click(object sender, RoutedEventArgs e)
        {
            var pane = new TimelineSample(Tokens.First());
            Panes.Add(pane);
            panesControll.Items.Add(pane);
            await pane.LoadTimeline();
            return;
        }
    }
}
