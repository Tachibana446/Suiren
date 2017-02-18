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
        }

        private void ChangeBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog() { Title = "ブラウザ設定", Filter = "すべてのファイル|*.*", DereferenceLinks = false };
            if (dialog.ShowDialog() == true)
            {
                Setting.Instance.BrowserPath = dialog.FileName;
            }

        }

        private void DefaultBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.BrowserPath = "";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.RollBack();
            Close();
        }

        private void ChangeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = "画像を選択",
                Filter = "画像|*.png;*.jpg;*.bmp;*.gif|すべてのファイル|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                Setting.Instance.BackgroundImagePath = dialog.FileName;
            }
        }

        private void ResetBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.BackgroundImagePath = "";
        }
    }
}
