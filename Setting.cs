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
        /// ホームの背景に出す画像
        /// </summary>
        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { if (_backgroundImagePath != value) { _backgroundImagePath = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundImagePath))); } }
        }
        private string _backgroundImagePath = "";
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
        /// 各ペインの色
        /// </summary>
        public ObservableCollection<PaneColor> PaneColors { get; set; } = new ObservableCollection<PaneColor>();

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
            PaneColors.Add(new PaneColor(typeof(TimelineSample)));
            PaneColors.Add(new PaneColor(typeof(MentionsTimeline), Brushes.LightPink));
        }

        private Setting(Setting s)
        {
            this.BrowserPath = s.BrowserPath;
            this.BackgroundImagePath = s.BackgroundImagePath;
            this.PaneOpacity = s.PaneOpacity;
            this.PaneColors = s.PaneColors;
        }

        /// <summary>
        /// ペインのクラスとカラーを保持する
        /// </summary>
        public class PaneColor : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public Type PaneClass { get; private set; }
            public Brush Color
            {
                get { return _color; }
                set
                {
                    if (_color != value)
                    {
                        _color = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
                    }
                }
            }
            private Brush _color;
            public PaneColor(Type paneClass, Brush color = null)
            {
                if (color == null) color = Brushes.White;
                PaneClass = paneClass;
                Color = color;
            }
        }
    }
}
