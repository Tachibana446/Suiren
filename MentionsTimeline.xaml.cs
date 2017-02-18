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
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (autoLoad.IsChecked)
                await LoadTimeline();
        }

        public async Task LoadTimeline()
        {
            var tl = await token.Statuses.MentionsTimelineAsync();
            tl.OrderBy(t => t.CreatedAt);
            foreach (var status in tl)
            {
                if (!Timeline.Any(t => t.Tweet.Id == status.Id))
                    Timeline.Insert(0, new TweetPanel(new Tweet(status), parent));
            }
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTimeline();
        }

    }
}
