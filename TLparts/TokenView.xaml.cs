using CoreTweet;
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

namespace Suiren.TLparts
{
    /// <summary>
    /// TokenView.xaml の相互作用ロジック
    /// </summary>
    public partial class TokenView : UserControl
    {
        private User user;

        public TokenView(User user)
        {
            InitializeComponent();
            this.user = user;
            dockPanel.DataContext = user;
        }
    }
}
