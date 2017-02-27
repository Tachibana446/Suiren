using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using System.IO;

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

        /// <summary>
        /// テキスト、画像をHTMLファイルにまとめて保存する。保存したHTMLファイルのFileInfoが帰る
        /// </summary>
        /// <returns></returns>
        public FileInfo Save()
        {
            FileInfo html = null;
            var files = SaveMedias();
            if (File.Exists("medias/template/template.html"))
            {
                var text = string.Join("\n", File.ReadLines("medias/template/template.html"));
                text = text.Replace("<<Icon>>", status.User.ProfileImageUrl);
                text = text.Replace("<<Name>>", UserNameAndScreenName);
                text = text.Replace("<<DateTime>>", $@"<a href=""https://twitter.com/{status.User.ScreenName}/status/{Id}"">{CreatedAtStr}</a>");
                if (isRetweet)
                {
                    FileInfo original = RetweetTweet.Save();
                    string iframe = $@"<iframe src=""./{original.Name}"" ></iframe>";
                    string retweetFrom = $@"<a href=""./{original.Name}"">RT元のツイート</a>";
                    text = text.Replace("<<Text>>", iframe + "<br>" + retweetFrom);
                    text = text.Replace("<<Medias>>", "");
                }
                else
                {
                    text = text.Replace("<<Text>>", Text.Replace("\n", "<br>"));
                    var mediasStr = "";
                    foreach (var file in files)
                    {
                        mediasStr += $@"<a href=""./{file.Name}""><img class=""ui large image"" src=""./{file.Name}""></a><br>";
                    }
                    text = text.Replace("<<Medias>>", mediasStr);
                }
                var filePath = isRetweet ? $"medias/RT_{Id}.html" : $"medias/{Id}.html";
                html = new FileInfo(filePath);
                using (var sw = new StreamWriter(html.FullName))
                    sw.WriteLine(text);
            }
            return html;
        }

        /// <summary>
        /// ツイート中のメディアを保存する
        /// </summary>
        /// <returns></returns>
        private List<FileInfo> SaveMedias()
        {
            var medias = new List<FileInfo>();
            int i = 0;
            if (Entities != null && Entities.Media != null && Entities.Media.Length > 0)
            {
                foreach (var ent in Entities.Media)
                {
                    if (ent.VideoInfo != null)
                    {
                        foreach (var v in ent.VideoInfo.Variants)
                        {
                            var finfo = new FileInfo($"./medias/{status.Id}_{i}{Path.GetExtension(v.Url)}");
                            i++;
                            using (var wc = new System.Net.WebClient())
                            {
                                wc.DownloadFileAsync(new Uri(v.Url), finfo.FullName);
                            }
                            medias.Add(finfo);
                        }
                    }
                    else
                    {
                        if (ent.MediaUrl == null) continue;
                        var finfo = new FileInfo($"./medias/{status.Id}_{i}{Path.GetExtension(ent.MediaUrl)}");
                        i++;
                        using (var wc = new System.Net.WebClient())
                            wc.DownloadFileAsync(new Uri(ent.MediaUrl), finfo.FullName);
                        medias.Add(finfo);
                    }
                }
            }
            return medias;
        }
    }
}
