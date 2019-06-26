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
using System.IO;

namespace CopyPaster.Controls
{
    /// <summary>
    /// Interaction logic for MultiClipboardItem.xaml
    /// </summary>
    public partial class MultiClipboardItem : UserControl
    {
        public string ClipboardContent { get; set; }
        public string Keybind { get; set; }

        public bool EditMode { get; set; } = false;

        public event EventHandler<ClipboardItemArgs> Save;
        public event EventHandler<ClipboardItemArgs> Delete;
        public event EventHandler<ClipboardItemArgs> Copy;
        public event EventHandler<ClipboardItemArgs> Edit;

        public MultiClipboardItem()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditButton.IsChecked != null && (bool)EditButton.IsChecked)
            {
                EditMode = true;
                //Clipboard.SetImage(System.Drawing.Image.FromFile(""));
                //Process
                
            }
            else
            {
                EditMode = false;
                CopyButton.IsHitTestVisible = true;

                Edit?.Invoke(this,
                    new ClipboardItemArgs { ClipboardContent = ClipboardContent, Keybind = Keybind });
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save?.Invoke(sender,
                new ClipboardItemArgs { ClipboardContent = ClipboardContent, Keybind = Keybind });
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Delete?.Invoke(this,
                new ClipboardItemArgs { ClipboardContent = ClipboardContent, Keybind = Keybind });
            Edit?.Invoke(this,
                new ClipboardItemArgs { ClipboardContent = ClipboardContent, Keybind = Keybind });
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button senderBtn = sender as Button;
            if (!EditMode)
            {
                senderBtn.Opacity = 1;
            }
            else
            {
                senderBtn.IsHitTestVisible = false;
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button senderBtn = sender as Button;
            senderBtn.Opacity = 0;
            if (!EditMode)
            {
                senderBtn.IsHitTestVisible = true;
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Copy?.Invoke(this,
               new ClipboardItemArgs { ClipboardContent = ClipboardContent, Keybind = Keybind });
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Edit?.Invoke(this,
                new ClipboardItemArgs { ClipboardContent = ClipboardContent, Keybind = Keybind });
        }
    }

    [Serializable]
    public class ClipboardItemArgs
    {
        public string ClipboardContent { get; set; }
        public string Keybind { get; set; }
    }
}
