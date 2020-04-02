using NotEnoughHotkeys.Data;
using NotEnoughHotkeys.Data.Types;
using NotEnoughHotkeys.Data.Types.Actions;
using NotEnoughHotkeys.Misc;
using NotEnoughHotkeys.SubprocessAPI;
using RawInput_dll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static NotEnoughHotkeys.Data.Constants;

namespace NotEnoughHotkeys.Forms
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsSettingKeyboard = false;
        private bool IsStartedAsAdmin = false;

        private RawInput rawInput;

        private NEHSubprocess UserSubprocess;
        private NEHSubprocess AdminSubprocess;


        public MainWindow()
        {
            InitializeComponent();
            MainWindowHandlers.Init(this);
        }

        private void ThisMainWindow_ContentRendered(object sender, EventArgs e)
        {
            macrosItemList.ItemsSource = Variables.Macros;
             
            IsStartedAsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            var Handle = new WindowInteropHelper(this).Handle;
            rawInput = new RawInput(Handle, false);
            rawInput.KeyPressed += new RawKeyboard.DeviceEventHandler(RawInputHandler);
        }

        private void RawInputHandler(object sender, RawInputEventArg e)
        {
            if (IsSettingKeyboard && e.KeyPressEvent.KeyPressState == KEYUP)
            {
                Data.Types.Keyboard kbd = Helper.GetKeyboardInfo(e.KeyPressEvent.DeviceName);
                kbd.HWID = e.KeyPressEvent.DeviceName;
                kbd.Name = e.KeyPressEvent.Name;
                Variables.TargetKeyboard = kbd;

                selectKeyboardBtn.Content = "Select";
                selectKeyboardBtn.IsHitTestVisible = true; //enable button click handler
                currentKeyboardLbl.Content = "Keyboard: " + e.KeyPressEvent.Name;
                currentKeyboardLbl.Foreground = Helper.GetFromResources<SolidColorBrush>("PrimaryForegroundAccent");
                kbdInfoBtn.IsEnabled = true;
                IsSettingKeyboard = false;
                
                UserSubprocess = new NEHSubprocess(false, kbd.HWID, PIPENAME);
                if (IsStartedAsAdmin)
                {
                    AdminSubprocess = new NEHSubprocess(true, kbd.HWID, PIPENAME_ADMIN);
                    AdminSubprocess.KeyEventRecieved += new NEHSubprocess.KeyEventRecievedHandler(KeyPressRecieved);
                    _ = AdminSubprocess.StartProcess();
                }
                UserSubprocess.KeyEventRecieved += new NEHSubprocess.KeyEventRecievedHandler(KeyPressRecieved);
                _ = UserSubprocess.StartProcess();
            }
        }

        private void KeyPressRecieved(object sender, NEHKeyPressEventArgs e)
        {
            if(e.State == NEHKeyState.KeyUp)
            {
                _ = HandleMacroAsync(e.KeyCode);
            }
        }

        private async Task HandleMacroAsync(int keycode)
        {
            Key key = KeyInterop.KeyFromVirtualKey(keycode);
            foreach(var macro in Variables.Macros)
            {
                if(macro.Hotkey == key)
                {
                    _ = macro.Action.PerformAsync();
                }
            }
            await Task.Delay(0);
        }

        private async void SelectKeyboardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserSubprocess != null) await UserSubprocess.StopProcess();
            if (AdminSubprocess != null) await AdminSubprocess.StopProcess();

            selectKeyboardBtn.Content = "Waiting...";
            selectKeyboardBtn.IsHitTestVisible = false; //disable button click handler
            currentKeyboardLbl.Content = "Press any key on the desired keyboard";
            currentKeyboardLbl.Foreground = new SolidColorBrush(Colors.Orange);
            IsSettingKeyboard = true;
        }

        private void KbdInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            KeyboardInfoWindow kiw = new KeyboardInfoWindow(Variables.TargetKeyboard);
            Clipboard.SetText(Variables.TargetKeyboard.HWID);
            kiw.Show();
        }

        private void MacrosItemList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            editMacroBtn.IsEnabled = !(macrosItemList.SelectedItems.Count > 1);
        }

        private void AddMacroBtn_Click(object sender, RoutedEventArgs e)
        {
            MacroEditWindow mew = new MacroEditWindow(null);
            mew.Show();
        }

        private void RemoveMacroBtn_Click(object sender, RoutedEventArgs e)
        {
            ((List<MacroItem>)macrosItemList.SelectedItems).ForEach(x => Variables.Macros.Remove(x));
        }

        private void EditMacroBtn_Click(object sender, RoutedEventArgs e)
        {
            MacroEditWindow mew = new MacroEditWindow((MacroItem)macrosItemList.SelectedItem);
            mew.Show();
        }

        private async void QuitAppBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserSubprocess != null) await UserSubprocess.StopProcess();
            if (AdminSubprocess != null) await AdminSubprocess.StopProcess();
            this.Close();
            Environment.Exit(0);
        }
    }
}
