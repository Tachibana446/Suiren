using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using CoreTweet;

namespace Suiren
{
    class TimelinePane
    {
        public List<Tweet> Timeline { get; set; } = new List<Tweet>();
        public bool isAutoLoad { get; set; } = false;

        private DockPanel panel = new DockPanel();
        public UserControl Controls { get; }

        private Tokens token { get; set; }

        public TimelinePane(Tokens token)
        {
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
    }
}
