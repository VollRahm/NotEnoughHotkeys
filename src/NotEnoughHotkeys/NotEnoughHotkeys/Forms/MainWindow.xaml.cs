using NotEnoughHotkeys.Data;
using NotEnoughHotkeys.Data.Types;
using NotEnoughHotkeys.Misc;
using NotEnoughHotkeys.RawInputLib;
using NotEnoughHotkeys.SubprocessAPI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static NotEnoughHotkeys.Data.Constants;

namespace NotEnoughHotkeys.Forms
{
    public partial class MainWindow : Window
    {
        private bool IsSettingKeyboard = false;
        private bool IsStartedAsAdmin = false;

        private RawInput rawInput;

        private NEHSubprocess UserSubprocess;
        private NEHSubprocess AdminSubprocess;

        private TrayIcon trayIcon;
        private SettingsWindow settingsWindow = new SettingsWindow();

        public MainWindow()
        {
            InitializeComponent();
            MainWindowHandlers.Init(this);

            IsStartedAsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            ConfigManager.InitPaths();
            LoadConfigs();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        private void ThisMainWindow_ContentRendered(object sender, EventArgs e)
        {
            macrosItemList.ItemsSource = Variables.Macros;
            
            if (!IsStartedAsAdmin)
            {
                var result = MessageBox.Show("Note: NotEnoughHotkeys was started without Admin permissions. It will only work partially and won't work inside processes with admin privileges. Do you want to restart as Admin? ", "Disclaimer", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if(result == MessageBoxResult.Yes)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                    psi.UseShellExecute = true;
                    psi.Verb = "runas";
                    Process.Start(psi);
                    Environment.Exit(0);
                }
            }
            var Handle = new WindowInteropHelper(this).Handle;
            rawInput = new RawInput();
            rawInput.RawKeyPressEvent += new RawInput.RawInputHandler(RawInputHandler);
            rawInput.Start();
            trayIcon = new TrayIcon(this);
            settingsWindow = new SettingsWindow();
        }

        private async void RawInputHandler(object sender, RawKeyPressEventArgs e)
        {
            if (IsSettingKeyboard && e.KeyState == KeyPressState.Up)
            {
                Data.Types.Keyboard kbd = new Data.Types.Keyboard();
                kbd.Description = e.Keyboard.Description;
                kbd.HWID = e.Keyboard.HWID;
                kbd.Name = e.Keyboard.Name;
                kbd.Layout = e.Keyboard.KeyboardLayout;
                Variables.Config.TargetKeyboard = kbd;

                selectKeyboardBtn.Content = "Select";
                selectKeyboardBtn.IsHitTestVisible = true; //enable button click handler
                currentKeyboardLbl.Content = "Keyboard: " + e.Keyboard.Name;
                currentKeyboardLbl.Foreground = Helper.GetFromResources<SolidColorBrush>("PrimaryForegroundAccent");
                kbdInfoBtn.IsEnabled = true;
                IsSettingKeyboard = false;

                await NEHSubprocess.KillAllProcesses();
                _ = StartSubprocesses(kbd);

                ConfigManager.StoreObject(Variables.Config, Constants.ConfigPath);
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
            KeyboardInfoWindow kiw = new KeyboardInfoWindow(Variables.Config.TargetKeyboard);
            Clipboard.SetText(Variables.Config.TargetKeyboard.HWID);
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
            macrosItemList.SelectedItems.OfType<MacroItem>().ToList().ForEach(x => Variables.Macros.Remove(x));
        }

        private void EditMacroBtn_Click(object sender, RoutedEventArgs e)
        {
            MacroEditWindow mew = new MacroEditWindow((MacroItem)macrosItemList.SelectedItem);
            mew.Show();
        }

        public async void QuitAppBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserSubprocess != null) await UserSubprocess.StopProcess();
            if (AdminSubprocess != null) await AdminSubprocess.StopProcess();
            AdminSubprocess = null;
            UserSubprocess = null;
            this.Close();
            Environment.Exit(0);
        }

        private async void hideWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            this.Hide();
            if (Variables.FirstEverStartup) await trayIcon.ShowNotification("Minimized", "NotEnoughHotkeys has been minimized and is available from its Tray Icon.", System.Windows.Forms.ToolTipIcon.Info, 1400);

        }

        private async void EnabledCb_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                trayIcon.IsHookEnabled = EnabledCb.IsChecked.Value;
                if (EnabledCb.IsChecked.Value)
                {
                    if (string.IsNullOrEmpty(Variables.Config.TargetKeyboard.HWID)) return;
                    await NEHSubprocess.KillAllProcesses();
                    UserSubprocess = new NEHSubprocess(false, Variables.Config.TargetKeyboard.HWID, PIPENAME);
                    if (IsStartedAsAdmin)
                    {
                        AdminSubprocess = new NEHSubprocess(true, Variables.Config.TargetKeyboard.HWID, PIPENAME_ADMIN);
                        AdminSubprocess.KeyEventRecieved += new NEHSubprocess.KeyEventRecievedHandler(KeyPressRecieved);
                        _ = AdminSubprocess.StartProcess();
                    }
                    UserSubprocess.KeyEventRecieved += new NEHSubprocess.KeyEventRecievedHandler(KeyPressRecieved);
                    _ = UserSubprocess.StartProcess();
                }
                else
                {
                    if (UserSubprocess != null) await UserSubprocess.StopProcess();
                    if (AdminSubprocess != null) await AdminSubprocess.StopProcess();
                    UserSubprocess = null;
                    AdminSubprocess = null;
                }
            }
            catch { }
        }

        private void LoadConfigs()
        {
            if (!File.Exists(Constants.ConfigPath))
            {
                Variables.FirstEverStartup = true;
                ConfigManager.StoreObject(Variables.Config, Constants.ConfigPath);
                return;
            }
            Variables.Config = ConfigManager.LoadFromFile<Config>(Constants.ConfigPath, new Config());
            Variables.Macros = ConfigManager.LoadFromFile<ObservableCollection<MacroItem>>(Constants.MacrosPath, new ObservableCollection<MacroItem>());

            if (!string.IsNullOrEmpty(Variables.Config.TargetKeyboard.HWID))
            {
                currentKeyboardLbl.Content = "Keyboard: " + Variables.Config.TargetKeyboard.Name;
                _ = StartSubprocesses(Variables.Config.TargetKeyboard);
                kbdInfoBtn.IsEnabled = true;
            }

        }

        private async void ThisMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (UserSubprocess != null) await UserSubprocess.StopProcess();
            if (AdminSubprocess != null) await AdminSubprocess.StopProcess();
            ConfigManager.StoreObject(Variables.Config, Constants.ConfigPath);
            ConfigManager.StoreObject(Variables.Macros, Constants.MacrosPath);
        }

        private async Task StartSubprocesses(Data.Types.Keyboard kbd)
        {
            UserSubprocess = new NEHSubprocess(false, kbd.HWID, PIPENAME);
            if (IsStartedAsAdmin)
            {
                AdminSubprocess = new NEHSubprocess(true, kbd.HWID, PIPENAME_ADMIN);
                AdminSubprocess.KeyEventRecieved += new NEHSubprocess.KeyEventRecievedHandler(KeyPressRecieved);
                _ = AdminSubprocess.StartProcess();
            }
            UserSubprocess.KeyEventRecieved += new NEHSubprocess.KeyEventRecievedHandler(KeyPressRecieved);
            await UserSubprocess.StartProcess();
        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow.Show();
        }

        private void ThisMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Environment.GetCommandLineArgs().Contains("--minimized"))
            {
                this.Hide();
                this.ShowInTaskbar = true;
            }
        }
    }
}
