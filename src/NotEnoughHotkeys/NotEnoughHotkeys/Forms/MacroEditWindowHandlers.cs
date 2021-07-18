using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotEnoughHotkeys.Forms
{
    public static class MacroEditWindowHandlers
    {
        private static MacroEditWindow w;

        public static void Init(MacroEditWindow _w)
        {
            w = _w;
            w.MouseLeftButtonDown += new MouseButtonEventHandler(DragMoveHandler);
            w.sKc_KeycodeTb.TextChanged += new TextChangedEventHandler(UpdateKeycodeName);
        }

        private static void UpdateKeycodeName(object sender, TextChangedEventArgs e)
        {
            try
            {
                var key = KeyInterop.KeyFromVirtualKey(int.Parse(w.sKc_KeycodeTb.Text));
                //please ignore the following line of code
                w.sKc_keysCb.SelectedItem = w.sKc_keysCb.Items.Cast<object>().ToList().Where(x => x.ToString() == key.ToString()).FirstOrDefault();
            }
            catch { }
        }

        private static void DragMoveHandler(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(w.TopBar);
            if (pt.Y < w.TopBar.ActualHeight) w.DragMove();
        }
    }
}
