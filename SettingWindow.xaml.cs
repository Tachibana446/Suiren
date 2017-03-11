using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Suiren
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            Setting.Instance.BackUp();
            InitializeComponent();
            DataContext = Setting.Instance;
            panesComboBox1.ItemsSource = Setting.Instance.PaneBrushes.Select(pc => pc.PaneTitle);
            panesComboBox1.SelectedIndex = 0;
            homeBackGroundRect.Fill = Setting.Instance.BackgroundBrush;
        }

        /// <summary>
        /// ブラウザ変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Title = "ブラウザ設定", Filter = "すべてのファイル|*.*", DereferenceLinks = false };
            if (dialog.ShowDialog() == true)
            {
                Setting.Instance.BrowserPath = dialog.FileName;
            }

        }
        /// <summary>
        /// ブラウザを既定に
        /// </summary>
        private void DefaultBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.BrowserPath = "";
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// キャンセル　変更をロールバックして閉じる
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.RollBack();
            Close();
        }

        /// <summary>
        /// ホーム画面の背景を変更
        /// </summary>
        private void ChangeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new BrushPicker.BrushPickerWindow();

            if (dialog.ShowDialog() == true)
            {
                Setting.Instance.BackgroundBrush = dialog.NowBrush;
            }
        }
        /// <summary>
        /// ホーム画面の背景をデフォルトに
        /// </summary>
        private void ResetBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.BackgroundBrush = Brushes.White;
        }
        /// <summary>
        /// パネルの背景変更ボタンをクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paneColorChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var index = panesComboBox1.SelectedIndex;
            if (index == -1) return;
            var dialog = new BrushPicker.BrushPickerWindow(Setting.Instance.PaneBrushes[index].Brush);
            if (dialog.ShowDialog() == true)
            {
                Setting.Instance.PaneBrushes[index].Brush = dialog.NowBrush;
            }
        }
        /// <summary>
        /// パネルの背景を既定に
        /// </summary>
        private void paneColorDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            var index = panesComboBox1.SelectedIndex;
            if (index == -1) return;
            Setting.Instance.PaneBrushes[index].Brush = Brushes.White;
        }
    }
}
