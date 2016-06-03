using PictManager.Model;
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

namespace PictManager.View
{
    /// <summary>
    /// PreviewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public PreviewWindow(PictureInfo pi)
        {
            InitializeComponent();
            this.info = pi;
        }

        private PictureInfo info;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.info;

            
        }
    }
}
