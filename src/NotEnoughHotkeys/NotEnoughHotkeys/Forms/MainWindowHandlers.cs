﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NotEnoughHotkeys.Forms
{
    public static class MainWindowHandlers
    {
        private static MainWindow w;

        public static void Init(MainWindow _w)
        {
            if (Environment.GetCommandLineArgs().Contains("--minimized"))
            {
                _w.ShowInTaskbar = false;
                _w.WindowState = WindowState.Minimized;
            }
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
