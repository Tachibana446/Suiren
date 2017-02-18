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
using System.Windows.Shapes;

namespace Suiren
{
    /// <summary>
    /// AuthWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AuthWindow : Window
    {
        private OAuth.OAuthSession s;
        public string Pin { get; private set; }
        public bool isOk { get; private set; } = false;

        public AuthWindow(OAuth.OAuthSession session)
        {
            s = session;
            InitializeComponent();
            link.Text = s.AuthorizeUri.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Pin = textBox.Text;
            isOk = true;
            Close();
        }

        private void urlButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.BrowserStart(s.AuthorizeUri.ToString());
        }
    }
}
