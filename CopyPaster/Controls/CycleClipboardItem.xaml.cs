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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CopyPaster.Controls
{
    /// <summary>
    /// Interaction logic for CycleClipboardItem.xaml
    /// </summary>
    public partial class CycleClipboardItem : UserControl
    {
        public string ClipboardContent { get; set; }

        public event EventHandler<ClipboardItemArgs> Delete;

        public CycleClipboardItem()
        {
            InitializeComponent();
            DataContext = this;

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Delete?.Invoke(this,
                new ClipboardItemArgs { ClipboardContent = ClipboardContent });
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
