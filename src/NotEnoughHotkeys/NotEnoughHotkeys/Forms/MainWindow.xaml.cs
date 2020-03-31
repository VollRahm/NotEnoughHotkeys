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
            ObservableCollection<MacroItem> macros = new ObservableCollection<MacroItem>();
            macros.Add(new MacroItem(Key.A,new LaunchProcessMacro("Chrome", null)));
            macrosItemList.ItemsSource = macros;
        }

        private void ThisMainWindow_ContentRendered(object sender, EventArgs e)
        {
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
                
            }
        }

        private bool blockNextKeystroke = false;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WH_HOOK) //if the message is from the Keyboard Hook dll
            {
                if (IsSettingKeyboard) return new IntPtr(1);

            }
            return IntPtr.Zero;
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
            kiw.Show();
        }
    }
}
