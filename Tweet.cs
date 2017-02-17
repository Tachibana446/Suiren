using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;

namespace Suiren
{
    public class Tweet
    {
        private Status status { get; set; }

        public string IconUrl { get { return status.User.ProfileImageUrl; } }
        public string UserName { get { return status.User.Name; } }
        public string UserScreenName { get { return status.User.ScreenName; } }
        public string UserNameAndScreenName { get { return $"{UserName} (@{UserScreenName})"; } }
        public string Text { get { return status.Text; } }
        public long Id { get { return status.Id; } }

        public Tweet(Status s)
        {
            status = s;
        }
    }
}
