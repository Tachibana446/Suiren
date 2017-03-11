using CoreTweet;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.UI.Notifications;

namespace Suiren
{
    /// <summary>
    /// MentionsTimeline.xaml の相互作用ロジック
    /// </summary>
    public partial class MentionsTimeline : UserControl, Timeline
    {
        public ObservableCollection<TweetPanel> Timeline { get; set; } = new ObservableCollection<TweetPanel>();

        private Tokens token;
        private MainWindow parent;

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);

        public MentionsTimeline(Tokens t, MainWindow parent)
        {
            token = t;
            this.parent = parent;
            InitializeComponent();
            tweetsControl.ItemsSource = Timeline;
            timer.Interval = new TimeSpan(0, 5, 0);
            timer.Tick += Timer_Tick;
            timer.Start();
            // Layout
            Opacity = Setting.Instance.PaneOpacity;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (autoLoad.IsChecked)
                await LoadTimeline();
        }

        public async Task LoadTimeline()
        {
            var response = await token.Statuses.MentionsTimelineAsync(include_entities: true);
            var tl = response.OrderBy(t => t.CreatedAt);
            foreach (var status in tl)
            {
                if (!Timeline.Any(t => t.Tweet.Id == status.Id))
                {
                    var tweet = new Tweet(status);
                    Timeline.Insert(0, new TweetPanel(tweet, parent));
                    // トースト通知
                    ToastTweet(tweet);
                }
            }
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTimeline();
        }

        /// <summary>
        /// ツイートをアクションセンターにトースト表示
        /// </summary>
        /// <param name="t"></param>
        private void ToastTweet(Tweet t)
        {
            if (parent.IsActive) return;
            var tmpl = ToastTemplateType.ToastImageAndText02;
            var xml = ToastNotificationManager.GetTemplateContent(tmpl);
            var image = xml.GetElementsByTagName("image").First();
            var src = image.Attributes.GetNamedItem("src");
            src.InnerText = t.SentUserIcon;
            var text = xml.GetElementsByTagName("text");
            text[0].AppendChild(xml.CreateTextNode(t.UserScreenName + "からの返信"));
            text[1].AppendChild(xml.CreateTextNode(t.Text));
            var toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier("Suiren").Show(toast);
        }
    }
}
