using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NotEnoughHotkeys.Forms
{
    public static class MainWindowHandlers
    {
        private static MainWindow w;

        public static void Init(MainWindow _w)
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
