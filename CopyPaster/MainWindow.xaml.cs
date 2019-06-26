using CopyPaster.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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

namespace CopyPaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Multi-Clipboard
        private List<ClipboardItemArgs> clipboardItems = new List<ClipboardItemArgs>();
        private MultiClipboardSettings clipboardSettings = new MultiClipboardSettings();
        private Dictionary<HotKey, string> keybinds = new Dictionary<HotKey, string>();
        private int currentIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            if (clipboardSettings.ClipboardItems == null)
            {
                clipboardSettings.ClipboardItems = new List<ClipboardItemArgs>();
                clipboardSettings.Save();
            }
        }

        private void MultiClipboardList_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (HotKey hk in keybinds.Keys)
            {
                hk.Unregister();
                hk.Dispose();
            }

            MultiClipboardList.Children.Clear();
            keybinds.Clear();
            foreach (ClipboardItemArgs args in clipboardSettings.ClipboardItems)
            {
                MultiClipboardItem item = new MultiClipboardItem
                {
                    ClipboardContent = args.ClipboardContent,
                    Keybind = args.Keybind
                };

                item.Delete += MultiClipboardItem_Delete;
                item.Copy += MultiClipboardItem_Copy;
                item.Edit += MultiClipboardItem_Edit;
                MultiClipboardList.Children.Add(item);
                if (!String.IsNullOrWhiteSpace(item.Keybind)) keybinds.Add(BindKeys(item.Keybind), item.ClipboardContent);
            }

            new HotKey(Key.M, KeyModifier.Ctrl, Keybind_CycleClipboard);
        }

        private void MultiClipboardList_Save(object sender, RoutedEventArgs e)
        {
            
        }

        private void MultiClipboardList_Reload(object sender, RoutedEventArgs e)
        {
            foreach (HotKey hk in keybinds.Keys)
            {
                hk.Unregister();
                hk.Dispose();
            }

            keybinds.Clear();
            foreach (ClipboardItemArgs args in clipboardSettings.ClipboardItems)
            {
                MultiClipboardItem item = new MultiClipboardItem
                {
                    ClipboardContent = args.ClipboardContent,
                    Keybind = args.Keybind
                };

                if (item.Keybind != "") keybinds.Add(BindKeys(item.Keybind), item.ClipboardContent);
            }
        }

        private void Reload()
        {
            MultiClipboardList.Children.Clear();
            foreach (ClipboardItemArgs args in clipboardSettings.ClipboardItems)
            {
                MultiClipboardItem item = new MultiClipboardItem
                {
                    ClipboardContent = args.ClipboardContent,
                    Keybind = args.Keybind
                };
                
                item.Delete += MultiClipboardItem_Delete;
                item.Copy += MultiClipboardItem_Copy;
                item.Edit += MultiClipboardItem_Edit;
                MultiClipboardList.Children.Add(item);
                
            }
        }

        private void Save()
        {
            clipboardSettings.ClipboardItems.Clear();
            foreach (UIElement element in MultiClipboardList.Children)
            {
                if (element is MultiClipboardItem item)
                {
                    clipboardSettings.ClipboardItems.Add(new ClipboardItemArgs
                    {
                        ClipboardContent = item.ClipboardContent,
                        Keybind = item.Keybind,
                    });
                }
            }

            clipboardSettings.Save();
        }

        private HotKey BindKeys(string ksc)
        {
            ksc = ksc.ToLower();

            KeyBinding kb = new KeyBinding();
            KeyModifier km = new KeyModifier();

            if (ksc.Contains("alt"))
                km |= KeyModifier.Alt;
            if (ksc.Contains("shift"))
                km |= KeyModifier.Shift;
            if (ksc.Contains("ctrl") || ksc.Contains("ctl"))
                km |= KeyModifier.Ctrl;

            string key =
                ksc.Replace("+", "")
                    .Replace("-", "")
                    .Replace("_", "")
                    .Replace(" ", "")
                    .Replace("alt", "")
                    .Replace("shift", "")
                    .Replace("ctrl", "")
                    .Replace("ctl", "");

            key = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key);
            if (!string.IsNullOrEmpty(key))
            {
                KeyConverter k = new KeyConverter();
                kb.Key = (Key)k.ConvertFromString(key);
            }
            HotKey hk = new HotKey(kb.Key, km, Keybind_MultiClipboard);
            return hk;
        }

        private void Keybind_MultiClipboard(HotKey obj)
        {
            if (keybinds.ContainsKey(obj))
            {
                SetClipboardContent(keybinds[obj]);
            }
            else
            {
                // Keybind is not used anymore; dispose the hotkey.
                obj.Dispose();
            }
        }

        private void Keybind_CycleClipboard(HotKey obj)
        {
            if (CycleClipboardList.Children.Count < 1)
            {
                return;
            }

            if (currentIndex < CycleClipboardList.Children.Count)
            {
                CycleClipboardItem item = CycleClipboardList.Children[currentIndex] as CycleClipboardItem;
                try
                {
                    SetClipboardContent(item.ClipboardContent);
                }
                catch (ArgumentNullException)
                {
                    currentIndex = 0;
                    Keybind_CycleClipboard(obj);
                }
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
                Keybind_CycleClipboard(obj);
            }
        }

        private void SetClipboardContent(string content)
        {
            string[] contentParams = content.Split(',');
            if (contentParams[0] == ("loadScript") && Properties.Settings.Default.UseScripts)
            {
                List<string> scriptArgs = new List<string>();
                
                for (int i = 2; i < contentParams.Length; i++)
                {
                    scriptArgs.Add(contentParams[i]);
                }
                
                new ScriptLoader().LoadScript(contentParams[1], scriptArgs.ToArray());
            }
            else
            {
                Clipboard.SetText(content);
            }
            
        }

        private void MultiClipboardList_New(object sender, RoutedEventArgs e)
        {
            MultiClipboardItem item = new MultiClipboardItem();
            item.Delete += MultiClipboardItem_Delete;
            item.Copy += MultiClipboardItem_Copy;
            item.Edit += MultiClipboardItem_Edit;
            MultiClipboardList.Children.Add(item);
            clipboardSettings.ClipboardItems.Add(new ClipboardItemArgs { ClipboardContent = "", Keybind = ""});

            Save();
        }

        private void MultiClipboardItem_Delete(object sender, ClipboardItemArgs e)
        {
            // Check if it is MultiClipboardItem, but cast to UIElement
            if (sender is UIElement item && sender is MultiClipboardItem)
            {
                MultiClipboardList.Children.Remove(item);
            }
        }

        private void MultiClipboardItem_Copy(object sender, ClipboardItemArgs e)
        {
            SetClipboardContent(e.ClipboardContent);
        }

        private void MultiClipboardItem_Edit(object sender, ClipboardItemArgs e)
        {
            
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            Save();
        }

        private void CycleClipboardList_New(object sender, RoutedEventArgs e)
        {
            CycleClipboardItem item = new CycleClipboardItem();
            item.Delete += MultiClipboardItem_Delete;
            CycleClipboardList.Children.Add(item);
        }
        #endregion

        #region Settings
        #endregion
    }

    sealed class MultiClipboardSettings : ApplicationSettingsBase
    {
        [UserScopedSettingAttribute()]
        //[SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public List<ClipboardItemArgs> ClipboardItems
        {
            get { return (List<ClipboardItemArgs>)this["ClipboardItems"]; }
            set { this["ClipboardItems"] = value; }
        }
    }
}
