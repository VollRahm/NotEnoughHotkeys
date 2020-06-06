using NotEnoughHotkeys.Data;
using NotEnoughHotkeys.Data.Types;
using NotEnoughHotkeys.Data.Types.Actions;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotEnoughHotkeys.Forms
{
    /// <summary>
    /// Interaktionslogik für MacroEditWindow.xaml
    /// </summary>
    public partial class MacroEditWindow : Window
    {
        private MacroItem Macro;
        private bool IsEditWindow = false;

        public MacroEditWindow(MacroItem macro)
        {
            InitializeComponent();
            MacroEditWindowHandlers.Init(this);
            Macro = macro;
        }

        private void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            lp_adminTBtn.IsEnabled = IsAdmin;
            sKc_keysCb.ItemsSource = Enum.GetValues(typeof(Key)).Cast<Key>();

            if (Macro != null)
            {
                IsEditWindow = true;
                Title = "Edit Macro";
                nameTb.Text = Macro.Action.Name;
                hotkeyTb.Text = Macro.Hotkey.ToString();
                RadioButtonGrid.Children.Cast<RadioButton>().ToList().Where(x => (string)x.Content == Macro.Action.TypeName).First().IsChecked = true;

                
                switch (Macro.Action.TypeName)
                {
                    case "Launch Process":
                        ShowPanel(launchProcPanel);
                        FillLaunchProcPanel();
                        break;
                    case "Send Keystrokes":
                        ShowPanel(sendKeysPanel);
                        FillSendKeysPanel();
                        break;
                    case "Send Single Keycode":
                        ShowPanel(sendKeycodePanel);
                        FillSendKeycodePanel();
                        break;
                }
            }
            else
            {
                ((RadioButton)RadioButtonGrid.Children[0]).IsChecked = true;
            }
        }

       

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (launchProcRb.IsChecked.Value)
            {
                ShowPanel(launchProcPanel);
            }
            else if (sendKeysRb.IsChecked.Value)
            {
                ShowPanel(sendKeysPanel);
            }
            else if (sendKeycodeRb.IsChecked.Value)
            {
                ShowPanel(sendKeycodePanel);
            }
        }

        private void HidePanels()
        {
            launchProcPanel.Visibility = Visibility.Hidden;
            sendKeysPanel.Visibility = Visibility.Hidden;
            sendKeycodePanel.Visibility = Visibility.Hidden;
        }

        private void ShowPanel(UIElement panel)
        {
            HidePanels();
            panel.Visibility = Visibility.Visible;
        }

        private void FillLaunchProcPanel()
        {
            var action = (LaunchProcessMacro)Macro.Action;
            lp_procPathTb.Text = action.ProcessFileName;
            lp_procArgsTb.Text = action.ProcessArgs;
            lp_adminTBtn.IsChecked = action.LaunchAsAdmin;
            lp_procStartPathTb.Text = action.ProcessStartPath;
        }

        private void FillSendKeysPanel()
        {
            var action = (SendKeysMacro)Macro.Action;
            sK_KeystrokeTb.Text = action.Keystrokes;
        }

        private void FillSendKeycodePanel()
        {
            var action = (SendKeycodeMacro)Macro.Action;
            sKc_KeycodeTb.Text = action.KeyCode.ToString();
        }

        private void HotkeyTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            hotkeyTb.Text = e.Key.ToString();
        }

        private void ChoosePathBtn_Click(object sender, RoutedEventArgs e)
        {
            string InitalDir = Directory.GetCurrentDirectory();
            try
            {
                DirectoryInfo directory = new DirectoryInfo(lp_procPathTb.Text);
                if (Directory.Exists(directory.FullName))
                    InitalDir = directory.FullName;
                else if (File.Exists(directory.FullName))
                    InitalDir = new FileInfo(directory.FullName).DirectoryName;
            }
            catch { }

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = InitalDir,
                Filter = "Executables (*.exe)|*.exe|AHK-Scripts (*.ahk)|*.ahk|All files (*.*)|*.*",
                FilterIndex = 3,
                Multiselect = false
            };
            var result = ofd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                lp_procPathTb.Text = ofd.FileName;
            }
        }

        private void ChooseStartPathBtn_Click(object sender, RoutedEventArgs e)
        {
            string InitalDir = Directory.GetCurrentDirectory();
            try
            {
                string startDir = lp_procStartPathTb.Text;
                if (string.IsNullOrEmpty(startDir)) startDir = lp_procPathTb.Text;
                DirectoryInfo directory = new DirectoryInfo(lp_procStartPathTb.Text);
                if (Directory.Exists(directory.FullName))
                    InitalDir = directory.FullName;
                else if (File.Exists(directory.FullName))
                    InitalDir = new FileInfo(directory.FullName).DirectoryName;
            }
            catch { }

            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog ofd = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            { 
                SelectedPath = InitalDir
            };
            var result = ofd.ShowDialog();

            if (result.Value)
            {
                lp_procStartPathTb.Text = ofd.SelectedPath;
            }
            
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForEmtpyTextbox(nameTb)) return;
            if (CheckForEmtpyTextbox(hotkeyTb)) return;

            if (launchProcRb.IsChecked.Value)
            {
                if (CheckForEmtpyTextbox(lp_procPathTb)) return;
            }
            else if (sendKeysRb.IsChecked.Value)
            {
                if (CheckForEmtpyTextbox(sK_KeystrokeTb)) return;
            }
            else if (sendKeycodeRb.IsChecked.Value)
            {
                if (CheckForEmtpyTextbox(sKc_KeycodeTb)) return;
            }

            IMacroAction action = null;
            if (launchProcRb.IsChecked.Value)
            {
                action = new LaunchProcessMacro(nameTb.Text, lp_procPathTb.Text, lp_procArgsTb.Text, lp_procStartPathTb.Text , lp_adminTBtn.IsChecked.Value);
            }
            else if (sendKeysRb.IsChecked.Value)
            {
                action = new SendKeysMacro(nameTb.Text, sK_KeystrokeTb.Text);
            }else if (sendKeycodeRb.IsChecked.Value)
            {
                action = new SendKeycodeMacro(nameTb.Text, int.Parse(sKc_KeycodeTb.Text));
            }
            MacroItem item = new MacroItem((Key)Enum.Parse(typeof(Key), hotkeyTb.Text), action);
            
            if (IsEditWindow)
            {
                Variables.Macros[Variables.Macros.IndexOf(Macro)] = item;
            }
            else
                Variables.Macros.Add(item);
            ConfigManager.StoreObject(Variables.Macros, Constants.MacrosPath);
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool CheckForEmtpyTextbox(TextBox tb)
        {
            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.Focus();
                System.Media.SystemSounds.Exclamation.Play();
                return true;
            }
            else return false;
        }

        private void sKc_keysCb_Selected(object sender, RoutedEventArgs e)
        {
            if(!sKc_KeycodeTb.IsFocused)
            sKc_KeycodeTb.Text = KeyInterop.VirtualKeyFromKey((Key)sKc_keysCb.SelectedItem).ToString();
        }

        private static readonly Regex containsLetter = new Regex("[^0-9]+");
        private void sKc_KeycodeTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (containsLetter.IsMatch(e.Text))
            {
                e.Handled = true;
                System.Media.SystemSounds.Exclamation.Play();
            }
            else
            {
                int newNumber = int.Parse(sKc_KeycodeTb.Text + e.Text);
                if (newNumber > 254 || newNumber < 0)
                {
                    e.Handled = true;
                    System.Media.SystemSounds.Exclamation.Play();
                }
            }
        }

        private void sKc_KeycodeTb_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                if (containsLetter.IsMatch((String)e.DataObject.GetData(typeof(String))))
                {
                    e.CancelCommand();
                    System.Media.SystemSounds.Exclamation.Play();
                }
                else
                {
                    int newNumber = int.Parse(sKc_KeycodeTb.Text + (String)e.DataObject.GetData(typeof(String)));
                    if (newNumber > 254 || newNumber < 0)
                    {
                        e.Handled = true;
                        System.Media.SystemSounds.Exclamation.Play();
                    }
                }
            }
            else
            {
                
                e.CancelCommand();
            }
            
        }
    }
}
