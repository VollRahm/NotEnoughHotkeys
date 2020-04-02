using NotEnoughHotkeys.Data;
using NotEnoughHotkeys.Data.Types;
using NotEnoughHotkeys.Data.Types.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
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
        }

        private void HidePanels()
        {
            launchProcPanel.Visibility = Visibility.Hidden;
            sendKeysPanel.Visibility = Visibility.Hidden;
        }

        private void ShowPanel(UIElement panel)
        {
            HidePanels();
            panel.Visibility = Visibility.Visible;
        }

        private void FillLaunchProcPanel()
        {
            var action = (LaunchProcessMacro)Macro.Action;
            lp_procPathTb.Text = action.ProcessInfo.FileName;
            lp_procArgsTb.Text = action.ProcessInfo.Arguments;
            lp_adminTBtn.IsChecked = action.LaunchAsAdmin;
        }

        private void FillSendKeysPanel()
        {
            var action = (SendKeysMacro)Macro.Action;
            sK_KeystokeTb.Text = action.Keystrokes;
        }

        private void HotkeyTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            hotkeyTb.Text = e.Key.ToString();
        }

        private void ChoosePathBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
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
                if (CheckForEmtpyTextbox(sK_KeystokeTb)) return;
            }

            IMacroAction action = null;
            if (launchProcRb.IsChecked.Value)
            {
                action = new LaunchProcessMacro(nameTb.Text, new ProcessStartInfo(lp_procPathTb.Text, lp_procArgsTb.Text), lp_adminTBtn.IsChecked.Value);
            }
            else if (sendKeysRb.IsChecked.Value)
            {
                action = new SendKeysMacro(nameTb.Text, sK_KeystokeTb.Text);
            }
            MacroItem item = new MacroItem((Key)Enum.Parse(typeof(Key), hotkeyTb.Text), action);
            
            if (IsEditWindow)
            {
                Variables.Macros[Variables.Macros.IndexOf(Macro)] = item;
            }
            else
                Variables.Macros.Add(item);

            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool CheckForEmtpyTextbox(TextBox tb)
        {
            if (string.IsNullOrEmpty(nameTb.Text))
            {
                tb.Focus();
                System.Media.SystemSounds.Exclamation.Play();
                return true;
            }
            else return false;
        }
    }
}
