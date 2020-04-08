using NotEnoughHotkeys.Misc;
using System.Security.Principal;
using System.Windows;

namespace NotEnoughHotkeys.Forms
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private bool IsAdmin = false;

        public SettingsWindow()
        {
            InitializeComponent();
            SettingsWindowHandlers.Init(this);
        }

        private void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (IsAdmin)
            {
                autostartEnabledTb.IsChecked = AutoStart.Status(out bool asAdmin);
                autostartAsAdminCb.IsChecked = asAdmin;
            }
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdmin)
            {
                AutoStart.Toggle(autostartEnabledTb.IsChecked.Value, autostartAsAdminCb.IsChecked.Value);
            }
            this.Hide();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void autostartEnabledTb_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin)
            {
                if(autostartEnabledTb.IsChecked.Value)
                autostartEnabledTb.IsChecked = false;
                MessageBox.Show("Autostart can only be set when NotEnoughHotkeys is run with Admin privileges.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (autostartEnabledTb.IsChecked.Value)
                {
                    autostartAsAdminCb.IsEnabled = true;
                    autostartAsAdminCb.IsChecked = true;
                }
                else
                {
                    autostartAsAdminCb.IsEnabled = false;
                }
                
            }
        }
    }
}
