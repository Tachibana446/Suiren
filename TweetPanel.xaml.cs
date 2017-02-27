using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// TweetPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class TweetPanel : UserControl, INotifyPropertyChanged
    {
        private MainWindow parent;

        private List<string> MediaUrls = new List<string>();

        private Tweet _tweet;
        public Tweet Tweet
        {
            get
            {
                return _tweet;
            }
            set
            {
                if (_tweet != value)
                {
                    _tweet = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tweet)));
                }
            }
        }

        public TweetPanel(Tweet tweet, MainWindow parent)
        {
            this.parent = parent;
            Tweet = tweet;
            InitializeComponent();
            DataContext = Tweet;
            if (Tweet.isRetweet)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(Tweet.SentUserIcon);
                bmp.EndInit();
                RetweetUserIcon.Source = bmp;
            }
            else
            {
                RetweetUserIconBorder.Visibility = Visibility.Hidden;
            }
            var origTweet = Tweet.isRetweet ? Tweet.RetweetTweet : Tweet;
            if (origTweet.Entities != null && origTweet.Entities.Media != null && origTweet.Entities.Media.Length > 0)
            {
                foreach (var ent in origTweet.Entities.Media)
                {
                    if (ent.VideoInfo != null)
                    {
                        foreach (var v in ent.VideoInfo.Variants)
                            MediaUrls.Add(v.Url);
                    }
                    else
                    {
                        MediaUrls.Add(ent.MediaUrl + ":orig");
                    }
                }
                foreach (var url in MediaUrls)
                {
                    var bitmap = new BitmapImage(new Uri(url));
                    var button = new Button();
                    var image = new Image() { Width = 200, Height = 200, Source = bitmap };
                    button.Content = image;
                    button.Click += (s, e) => (new ImageWindow(url)).Show();
                    mediasWrapPanel.Children.Add(button);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void replyButton_Click(object sender, RoutedEventArgs e)
        {
            parent.ShowReplyWindow(this);
        }

        private void retweetButton_Click(object sender, RoutedEventArgs e)
        {
            parent.CreateRetweet(Tweet);
        }

        private void favButton_Click(object sender, RoutedEventArgs e)
        {
            parent.CreateFavorite(Tweet);
        }
    }
}
