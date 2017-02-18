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

        public TweetPanel(Tweet tweet)
        {
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
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
