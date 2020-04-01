using MaterialDesignThemes.Wpf;
using NotEnoughHotkeys.Data;
using NotEnoughHotkeys.Data.Types;
using NotEnoughHotkeys.Data.Types.Actions;
using NotEnoughHotkeys.KeyboardHook;
using NotEnoughHotkeys.Misc;
using RawInput_dll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        private RawInput rawInput;
      
        public MainWindow()
        {
            InitializeComponent();
            MainWindowHandlers.Init(this);
        }

        private void ThisMainWindow_ContentRendered(object sender, EventArgs e)
        {
            macrosItemList.ItemsSource = Variables.Macros;
            var handle = new WindowInteropHelper(this).Handle;
            rawInput = new RawInput(handle, false);
            rawInput.KeyPressed += new RawKeyboard.DeviceEventHandler(RawInputHandler);

            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc)); //override WndProc
            var result = NEHHook.StartHook(handle);
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
            }
            else
            {
                var ev = e.KeyPressEvent;
                if(ev.DeviceName == Variables.TargetKeyboard.HWID)
                {
                    blockNextKeystroke = true;
                    if(ev.KeyPressState == KEYUP)
                    {
                        _ = HandleMacroAsync(e.KeyPressEvent.VKey);
                    }
                }
                else
                {
                    blockNextKeystroke = false;
                }
            }
        }

        private bool blockNextKeystroke = false;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WH_HOOK) //if the message is from the Keyboard Hook dll
            {
                if (blockNextKeystroke || IsSettingKeyboard)
                {
                    Console.WriteLine("Blocking");
                    handled = true;
                    blockNextKeystroke = false;
                    return new IntPtr(-1);
                }

               
            }
            return IntPtr.Zero;
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

        private void selectKeyboardBtn_Click(object sender, RoutedEventArgs e)
        {
            selectKeyboardBtn.Content = "Waiting...";
            selectKeyboardBtn.IsHitTestVisible = false; //disable button click handler
            currentKeyboardLbl.Content = "Press any key on the desired keyboard";
            currentKeyboardLbl.Foreground = new SolidColorBrush(Colors.Orange);
            IsSettingKeyboard = true;
        }

        private void kbdInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            KeyboardInfoWindow kiw = new KeyboardInfoWindow(Variables.TargetKeyboard);
            Clipboard.SetText(Variables.TargetKeyboard.HWID);
            kiw.Show();
        }

        private void macrosItemList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            editMacroBtn.IsEnabled = !(macrosItemList.SelectedItems.Count > 1);
        }

        private void addMacroBtn_Click(object sender, RoutedEventArgs e)
        {
            MacroEditWindow mew = new MacroEditWindow(null, true);
            mew.Show();
        }
    }
}
