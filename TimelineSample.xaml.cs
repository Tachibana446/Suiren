﻿using System;
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

namespace Suiren
{
    /// <summary>
    /// TimelineSample.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineSample : UserControl
    {
        public List<Tweet> Timeline { get; set; } = new List<Tweet>();
        public bool isAutoLoad { get; set; } = false;

        private Tokens token;

        public TimelineSample(Tokens t)
        {
            token = t;
            InitializeComponent();
            tweetsControl.ItemsSource = Timeline;
        }

        public virtual async Task LoadTimeline()
        {
            var htl = await token.Statuses.HomeTimelineAsync();
            foreach (var status in htl)
            {
                if (!Timeline.Any(t => t.Id == status.Id))
                    Timeline.Add(new Tweet(status));
            }
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTimeline();
        }
    }
}
