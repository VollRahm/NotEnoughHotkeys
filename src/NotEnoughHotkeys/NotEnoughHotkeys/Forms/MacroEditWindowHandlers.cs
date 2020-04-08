using System.Windows;
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
        }

        private static void DragMoveHandler(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(w.TopBar);
            if (pt.Y < w.TopBar.ActualHeight) w.DragMove();
        }
    }
}
