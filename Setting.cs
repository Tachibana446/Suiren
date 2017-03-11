using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Suiren
{
    class Setting : INotifyPropertyChanged
    {
        public static Setting Instance { get; private set; } = new Setting();

        /// <summary>
        /// 開く時のブラウザ
        /// </summary>
        public string BrowserPath
        {
            get { return _browserPath; }
            set
            {
                if (_browserPath != value)
                {
                    _browserPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrowserPath)));
                }
            }
        }
        private string _browserPath = "";
        /// <summary>
        /// ホーム画面の背景
        /// </summary>
        public Brush BackgroundBrush { get; set; } = Brushes.White;

        /// <summary>
        /// 各ペインの透明度 1が一番濃い 0が透明
        /// </summary>
        public double PaneOpacity
        {
            get { return _paneOpacity; }
            set
            {
                if (_paneOpacity != value)
                {
                    _paneOpacity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaneOpacity)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaneOpacityPercent)));
                }
            }
        }
        private double _paneOpacity = 1.0;
        public string PaneOpacityPercent { get { return $"{PaneOpacity * 100}%"; } }

        private static Setting old = new Setting();
        /// <summary>
        /// 各ペインの背景
        /// </summary>
        public ObservableCollection<PaneBrush> PaneBrushes { get; set; } = new ObservableCollection<PaneBrush>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 変更前を保持
        /// </summary>
        public void BackUp()
        {
            old = new Setting(this);
        }
        /// <summary>
        /// キャンセル時、変更前に戻す
        /// </summary>
        public void RollBack()
        {
            Instance = old;
        }
        /// <summary>
        /// ブラウザが指定してあればそれで開き、なければデフォルトで開く
        /// </summary>
        /// <param name="url"></param>
        public void BrowserStart(string url)
        {
            if (BrowserPath == "")
                Process.Start(url);
            else
                Process.Start(BrowserPath, url);
        }

        private Setting()
        {
            // ペインの背景
            PaneBrushes.Add(new PaneBrush(typeof(TimelineSample), "ホーム"));
            PaneBrushes.Add(new PaneBrush(typeof(MentionsTimeline), "返信", Brushes.LightPink));
        }

        private Setting(Setting s)
        {
            this.BrowserPath = s.BrowserPath;
            this.BackgroundBrush = s.BackgroundBrush;
            this.PaneOpacity = s.PaneOpacity;
            this.PaneBrushes = s.PaneBrushes;
        }

        /// <summary>
        /// ペインのクラスとカラーを保持する
        /// </summary>
        public class PaneBrush : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public Type PaneClass { get; private set; }
            /// <summary>
            /// 表示する時の名前
            /// </summary>
            public string PaneTitle { get; private set; }
            public Brush Brush
            {
                get { return _brush; }
                set
                {
                    if (_brush != value)
                    {
                        _brush = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Brush)));
                    }
                }
            }
            private Brush _brush;
            public PaneBrush(Type paneClass, string name, Brush color = null)
            {
                if (color == null) color = Brushes.White;
                PaneClass = paneClass;
                PaneTitle = name;
                Brush = color;
            }
        }
    }
}
