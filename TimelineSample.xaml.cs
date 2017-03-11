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
using CoreTweet;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Suiren
{
    /// <summary>
    /// TimelineSample.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineSample : UserControl, Timeline
    {
        /// <summary>
        /// タイムラインに表示されているコントロールのリスト
        /// </summary>
        public ObservableCollection<Control> Timeline { get; set; } = new ObservableCollection<Control>();

        /// <summary>
        /// Tweetの一覧が必要になった時
        /// </summary>
        public List<TweetPanel> TweetPanels { get { return Timeline.Where(t => t.GetType() == typeof(TweetPanel)).Select(t => (TweetPanel)t).ToList(); } }
        /// <summary>
        /// このTLで使うトークン
        /// </summary>
        private Tokens token;
        /// <summary>
        /// 親のMainWindow
        /// </summary>
        private MainWindow parent;
        /// <summary>
        /// 自動読み込みのためのタイマー
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        /// <summary>
        /// 過去のツイートを読み込むボタン
        /// </summary>
        private TLparts.BackInTimeButton backInTimeButton = new TLparts.BackInTimeButton();
        /// <summary>
        /// 読み込んだ中で一番小さいID。これを与えることで過去のツイートを読み込める.
        /// </summary>
        private long minimumId = -1;

        public TimelineSample(Tokens t, MainWindow parent)
        {
            token = t;
            this.parent = parent;
            InitializeComponent();
            tweetsControl.ItemsSource = Timeline;
            timer.Interval = new TimeSpan(0, 5, 0);
            timer.Tick += Timer_Tick;
            timer.Start();
            backInTimeButton.Click += BackInTimeButton_Click;
            // Layout
            Opacity = Setting.Instance.PaneOpacity;
        }


        /// <summary>
        /// タイマーで繰り返すメソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (autoLoad.IsChecked)
                await LoadTimeline();
        }

        /// <summary>
        /// タイムライン読み込み
        /// </summary>
        /// <returns></returns>
        public async Task LoadTimeline()
        {
            var response = await token.Statuses.HomeTimelineAsync(include_entities: true);
            var tl = response.OrderBy(t => t.CreatedAt);
            foreach (var status in tl)
            {
                if (!TweetPanels.Any(t => t.Tweet.Id == status.Id))
                {
                    Timeline.Insert(0, new TweetPanel(new Tweet(status), parent));
                }
                if (status.Id < minimumId || minimumId == -1) minimumId = status.Id;
            }
            // 過去のツイートを読み込むボタン
            Timeline.Remove(backInTimeButton);
            Timeline.Add(backInTimeButton);
        }

        // 過去のツイートを遡る
        public async Task GoBackInTime()
        {
            backInTimeButton.IsEnabled = false;
            Timeline.Remove(backInTimeButton);
            var tl = (await token.Statuses.HomeTimelineAsync(include_entities: true, max_id: minimumId)).OrderBy(t => t.CreatedAt);
            foreach (var status in tl)
            {
                Timeline.Add(new TweetPanel(new Tweet(status), parent));
                if (status.Id < minimumId || minimumId == -1) minimumId = status.Id;
            }
            Timeline.Add(backInTimeButton);
            backInTimeButton.IsEnabled = true;
        }


        private async void BackInTimeButton_Click(object sender, RoutedEventArgs e)
        {
            await GoBackInTime();
        }

        /// <summary>
        /// ロードボタンを押すとタイムライン読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTimeline();
        }


    }
}
