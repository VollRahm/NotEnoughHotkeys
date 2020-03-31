using NotEnoughHotkeys.Data.Types;
using System.Windows;

namespace NotEnoughHotkeys.Forms
{
    /// <summary>
    /// Interaktionslogik für KeyboardInfoWindow.xaml
    /// </summary>
    public partial class KeyboardInfoWindow : Window
    {

        public KeyboardInfoWindow(Keyboard kbd)
        {
            InitializeComponent();
            KeyboardInfoWindowHandlers.Init(this);
            this.Title = kbd.Name;
            hwidLbl.Text += kbd.HWID;
            layoutLbl.Content += kbd.Layout;
            descLbl.Content += kbd.Description;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
