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

        /// <summary>
        ///  アイコン（RTならRT元の）
        /// </summary>
        public string IconUrl
        {
            get
            {
                return isRetweet ? RetweetTweet.IconUrl : status.User.ProfileImageUrl;
            }
        }
        /// <summary>
        /// アイコン
        /// </summary>
        public string SentUserIcon { get { return status.User.ProfileImageUrl; } }
        /// <summary>
        /// ツイートのユーザー名（RT元ではない）
        /// </summary>
        private string UserName { get { return status.User.Name; } }
        public string UserScreenName { get { return status.User.ScreenName; } }
        private string UserNameAndScreenName { get { return $"{UserName} (@{UserScreenName})"; } }
        /// <summary>
        /// ユーザー名（RTならRT元のユーザー名）
        /// </summary>
        public string ShowName
        {
            get
            {
                return isRetweet ? RetweetTweet.ShowName : UserNameAndScreenName;
            }
        }
        /// <summary>
        /// テキスト。RTならRT元のテキスト
        /// </summary>
        public string Text { get { return isRetweet ? RetweetTweet.Text : status.Text; } }
        public long Id { get { return status.Id; } }
        public DateTimeOffset CreatedAt { get { return status.CreatedAt; } }
        public bool isRetweet { get { return status.RetweetedStatus != null; } }
        public Tweet RetweetTweet { get { return new Tweet(status.RetweetedStatus); } }
        public long RetweetedId { get { return isRetweet ? RetweetTweet.Id : -1; } }

        public Entities Entities { get { return status.Entities; } }
        /// <summary>
        /// ツイート時間(RTならRT元の)
        /// </summary>
        public string CreatedAtStr
        {
            get
            {
                if (isRetweet)
                    return RetweetTweet.CreatedAtStr;
                else
                {
                    DateTime d = CreatedAt.DateTime.AddHours(9);
                    return $"{d.Year}/{d.Month}/{d.Day} {d.Hour}:{d.Minute}:{d.Second}";
                }
            }
        }
        private string SentAtStr
        {
            get
            {
                DateTime d = CreatedAt.DateTime.AddHours(9);
                return $"{d.Year}/{d.Month}/{d.Day} {d.Hour}:{d.Minute}:{d.Second}";
            }
        }

        public string SentUserText
        {
            get { return isRetweet ? $"{UserNameAndScreenName}がRTしました。\n({SentAtStr})" : ""; }
        }

        public Tweet(Status s)
        {
            status = s;
        }
    }
}
